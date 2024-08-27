using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.Post;
using BlogApi.Dtos.Tag;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Repositories {
    public class PostRepository : Repository<Post>, IPostRepository {
        public PostRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

        public async Task<(int TotalCount, List<CompactPostDto> compactPosts)> GetPaged(SelectionOptions selectionOptions) {
            IQueryable<Post> posts = _appDbContext.Set<Post>();
            if (selectionOptions.CanFilter("title")) {
                posts = posts.Where(p => p.Title.StartsWith(selectionOptions.FilterValue!));
            }
            // TODO: add filteration on creation at

            Expression<Func<Post, object>> keySelector;
            if (selectionOptions.OrderBy.Equals("title", StringComparison.CurrentCultureIgnoreCase)) keySelector = p => p.Title;
            else if (selectionOptions.OrderBy.Equals("createdat", StringComparison.CurrentCultureIgnoreCase))
                keySelector = p => p.CreatedAt;
            else keySelector = p => p.Id;

            if (selectionOptions.Order is not null && selectionOptions.Order.Equals("desc", StringComparison.CurrentCultureIgnoreCase))
                posts = posts.OrderByDescending(keySelector);
            else posts = posts.OrderBy(keySelector);

            var totalPosts = await posts.CountAsync();

            var result = await posts.Skip((selectionOptions.Page - 1) * selectionOptions.PageSize)
                .Take(selectionOptions.PageSize)
                .Select(p => new CompactPostDto {
                    Id = p.Id,
                    Title = p.Title,
                    CategoryId = p.Category.Id,
                    CategoryName = p.Category.Name,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    CreatedBy = new CreatedByUserDto {
                        Id = p.User.Id,
                        Email = p.User.Email,
                        Image = p.User.Image,
                        Username = p.User.Username
                    },
                    LastModification = p.LastModification,
                    TotalComments = p.TotalComments,
                    TotalReactions = p.TotalReactions,
                    Tags = p.PostTags.Select(pt => new TagDto { Id = pt.TagId, Name = pt.Tag.Name })
                }).ToListAsync();
            return (totalPosts, result);

        }

        public async Task<CompletePostDto?> GetComplete(int id) {
            var posts =  await _appDbContext.Set<Post>()
                .Where(post => post.Id == id)
                .Select(post => new CompletePostDto {
                    Id = post.Id,
                    Title = post.Title,
                    CategoryId = post.Category.Id,
                    CategoryName = post.Category.Name,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    CreatedBy = new CreatedByUserDto {
                        Id = post.User.Id,
                        Email = post.User.Email,
                        Image = post.User.Image,
                        Username = post.User.Username
                    },
                    LastModification = post.LastModification,
                    TotalComments = post.TotalComments,
                    TotalReactions = post.TotalReactions,
                    Tags = post.PostTags.Select(pt => new TagDto { Id = pt.TagId, Name = pt.Tag.Name }),
                    Comments = post.Comments.Select(comment => new CommentDto {
                        Id = comment.Id,
                        Content = comment.Content,
                        CreatedAt = comment.CreatedAt,
                        CommentedBy = new CreatedByUserDto {
                            Id = comment.User.Id,
                            Email = comment.User.Email,
                            Image = comment.User.Image,
                            Username = comment.User.Username
                        }
                    })
                }).ToListAsync();
            if (posts.Count == 0) return null;
            else return posts[0];
        }
        public async Task<Post?> GetWithComments(Expression<Func<Post, bool>> predicate) {
           return await _appDbContext.Set<Post>()
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(predicate);
        }
    }
}
