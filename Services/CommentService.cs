using BlogApi.Database;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;
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
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = new Comment { Content = createCommentDto.Content, PostId = createCommentDto.PostId.Value, UserId = user.Id };
            _appDbContext.Set<Comment>().Add(comment);
            post.TotalComments++;
            await _appDbContext.SaveChangesAsync();
            return new CommentDto { Id = comment.Id, Content = comment.Content, CommentedBy = user , CreatedAt = comment.CreatedAt};
        }

        public async Task<CommentDto> DeleteComment(int commentId) {
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = await _appDbContext.Set<Comment>()
                .Include(c => c.Post)
                .SingleOrDefaultAsync(c => c.Id == commentId);
            if (comment is null) throw new BadRequestException("There isn't a comment with the provided id!");
            if (comment.UserId != user.Id) throw new BadRequestException("Can't delete the comment");
            comment.Post.TotalComments--;
            _appDbContext.Set<Comment>().Remove(comment);
            await _appDbContext.SaveChangesAsync();
            return new CommentDto { Id = comment.Id, CommentedBy = user, Content = comment.Content, CreatedAt = comment.CreatedAt };
        }

        public async Task<CommentDto> UpdateComment(UpdateCommentDto updateCommentDto) {
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = await _appDbContext.Set<Comment>()
                .Include(c => c.Post)
                .SingleOrDefaultAsync(c => c.Id == updateCommentDto.CommentId);
            if (comment is null) throw new BadRequestException("There isn't a comment with the provided id!");
            if (comment.UserId != user.Id) throw new BadRequestException("Can't update the comment");
            comment.Content = updateCommentDto.Content;
            await _appDbContext.SaveChangesAsync();
            return new CommentDto { Id = comment.Id, Content = comment.Content, CommentedBy = user, CreatedAt = comment.CreatedAt };
        }
    }
}
