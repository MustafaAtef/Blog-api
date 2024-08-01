using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.Post;
using BlogApi.Dtos.Tag;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Services {
    public class PostService : IPostService {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CompletePostDto> CreatePost(CreatePostDto createPostDto) {
            Category? category = await _checkCategory(createPostDto.CategoryId);
            if (category is null) throw new BadRequestException("There isn't a category with the provided id!");
            var tags = await _getTagsFromIdsAsync(createPostDto.Tags);
            if (tags.Count != createPostDto.Tags.Count)
                throw new BadRequestException("There aren't tags with the provided ids");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var post = new Post() {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CategoryId = category.Id,
                CreatedBy = user.Id,
                PostTags = new List<PostTag>()
            };
            foreach (var tagId in createPostDto.Tags) {
                post.PostTags.Add(new PostTag { Post = post, TagId = tagId });
            }
            _appDbContext.Set<Post>().Add(post);
            await _appDbContext.SaveChangesAsync();
            return new CompletePostDto { Tags = tags, Title = post.Title, Id = post.Id, CategoryId = post.CategoryId, CategoryName = category.Name, Content = post.Content, CreatedAt = post.CreatedAt, LastModification = post.LastModification, CreatedBy = user, TotalComments = post.TotalComments, TotalReactions = post.TotalReactions, Comments = new List<CommentDto>() };
        }


        public async Task<PageList<CompactPostDto>> GetAllPosts(SelectionOptions selectionOptions) {
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
                    CategoryId= p.Category.Id,
                    CategoryName= p.Category.Name,
                    Content= p.Content,
                    CreatedAt= p.CreatedAt,
                    CreatedBy = new CreatedByUserDto { 
                        Id =p.User.Id,
                        Email = p.User.Email,
                        Image= p.User.Image,
                        Username = p.User.Username 
                    },
                    LastModification = p.LastModification,
                    TotalComments = p.TotalComments,
                    TotalReactions = p.TotalReactions,
                    Tags = p.PostTags.Select(pt => new TagDto { Id = pt.TagId, Name = pt.Tag.Name})
                }).ToListAsync();
            return new PageList<CompactPostDto>(result, selectionOptions.Page, selectionOptions.PageSize, totalPosts);
        }

        public async Task<CompletePostDto> GetPostById(int id) {
            var res = await _appDbContext.Set<Post>()
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
            if (res is null) throw new BadRequestException("There isn't a post with the provided id!");
            return res[0];
        }

        public async Task<CompletePostDto> UpdatePost(UpdatePostDto updatePostDto) {
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var post = await _appDbContext.Set<Post>()
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .SingleOrDefaultAsync(p => p.Id == updatePostDto.Id);
            if (post.CreatedBy != user.Id) throw new NotAuthorizedException();
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var category = await _checkCategory(updatePostDto.CategoryId);
            if (category is null) throw new BadRequestException("There isn't a category with the provided id!");
            var tags = await _getTagsFromIdsAsync(updatePostDto.Tags);
            if (tags.Count != updatePostDto.Tags.Count) 
                throw new BadRequestException("There aren't tags with the provided ids");


            await _appDbContext.Set<PostTag>().Where(pt => pt.PostId == updatePostDto.Id).ExecuteDeleteAsync();
            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            post.Category = category;
            post.LastModification = DateTime.UtcNow;
            post.PostTags = new List<PostTag>();
            foreach (var tagId in updatePostDto.Tags) {
                post.PostTags.Add(new PostTag { Post = post, TagId = tagId });
            }
            await _appDbContext.SaveChangesAsync();

            return new CompletePostDto { Tags = tags, Title = post.Title, Id = post.Id, CategoryId = post.CategoryId, CategoryName = category.Name, Content = post.Content, CreatedAt = post.CreatedAt, LastModification = post.LastModification, CreatedBy = user, TotalComments = post.TotalComments, TotalReactions = post.TotalReactions,
              Comments = post.Comments
                .Select(comment => new CommentDto {
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
            };
        }
        private Task<List<TagDto>> _getTagsFromIdsAsync(IList<int>? tags) {
            return _appDbContext.Set<Tag>()
                .Where(tag => tags.Contains(tag.Id))
                .Select(tag => new TagDto { Id = tag.Id, Name = tag.Name })
                .ToListAsync();
        }
        private Task<Category?> _checkCategory(int? categoryId) {
            if (categoryId is null) return null;
            return _appDbContext.Set<Category>().SingleOrDefaultAsync(category => category.Id == categoryId);
        }

    }
}
