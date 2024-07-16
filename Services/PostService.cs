using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.Post;
using BlogApi.Dtos.Tag;
using BlogApi.Dtos.User;
using BlogApi.Entities;
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
            Category? category = await CheckCategory(createPostDto.CategoryId.Value);
            if (category is null) ArgumentNullException.ThrowIfNull(category);
            var tags = new List<TagDto>();
            if (createPostDto.Tags is not null) {
                tags = await _appDbContext.Set<Tag>().Where(tag => createPostDto.Tags.Contains(tag.Id))
                    .Select(tag => new TagDto { Id = tag.Id, Name = tag.Name }).ToListAsync();
                if (tags.Count != createPostDto.Tags.Count) throw new ArgumentException();
            }


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

        private async Task<Category?> CheckCategory(int categoryId) {
            return await _appDbContext.Set<Category>().SingleOrDefaultAsync(category => category.Id == categoryId);
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
            if (res is null) throw new ArgumentException();
            return res[0];
        }

        public Task<CompletePostDto> UpdatePost(UpdatePostDto updatePostDto) {
            throw new NotImplementedException();
        }
    }
}
