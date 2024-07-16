using BlogApi.Database;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogApi.Services {
    public class CommentService : ICommentService {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentService(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor) {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommentDto> CreateComment(CreateCommentDto createCommentDto) {
            var post = await _appDbContext.Set<Post>().SingleOrDefaultAsync(p => p.Id == createCommentDto.PostId);
            ArgumentNullException.ThrowIfNull(post);
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = new Comment { Content = createCommentDto.Content, PostId = createCommentDto.PostId.Value, UserId = user.Id };
            _appDbContext.Set<Comment>().Add(comment);
            post.TotalComments++;
            await _appDbContext.SaveChangesAsync();
            return new CommentDto { Id = comment.Id, Content = comment.Content, CommentedBy = user , CreatedAt = comment.CreatedAt};
        }

        public async Task<CommentDto> UpdateComment(UpdateCommentDto updateCommentDto) {
            var comment = await _appDbContext.Set<Comment>().SingleOrDefaultAsync(c => c.Id == updateCommentDto.CommentId);
            ArgumentNullException.ThrowIfNull(comment);
            comment.Content = updateCommentDto.Content;
            await _appDbContext.SaveChangesAsync();
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            return new CommentDto { Id = comment.Id, Content = comment.Content, CommentedBy = user, CreatedAt = comment.CreatedAt };
        }
    }
}
