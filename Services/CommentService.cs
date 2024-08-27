using BlogApi.Database;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.Repositories.Interfaces;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.Design;
using System.Security.Claims;

namespace BlogApi.Services {
    public class CommentService : ICommentService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommentService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommentDto> CreateComment(CreateCommentDto createCommentDto) {
            var post = await _unitOfWork.PostRepository.Get(p => p.Id == createCommentDto.PostId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = new Comment { Content = createCommentDto.Content, PostId = createCommentDto.PostId.Value, UserId = user.Id };
            _unitOfWork.CommentRepository.Add(comment);
            post.TotalComments++;
            await _unitOfWork.Complete();
            return new CommentDto { Id = comment.Id, Content = comment.Content, CommentedBy = user , CreatedAt = comment.CreatedAt};
        }

        public async Task<CommentDto> DeleteComment(int commentId) {
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = await _unitOfWork.CommentRepository.GetWithPost(c => c.Id == commentId);
            if (comment is null) throw new BadRequestException("There isn't a comment with the provided id!");
            if (comment.UserId != user.Id) throw new NotAuthorizedException();
            comment.Post.TotalComments--;
            _unitOfWork.CommentRepository.Remove(comment);
            await _unitOfWork.Complete();
            return new CommentDto { Id = comment.Id, CommentedBy = user, Content = comment.Content, CreatedAt = comment.CreatedAt };
        }

        public async Task<CommentDto> UpdateComment(UpdateCommentDto updateCommentDto) {
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var comment = await _unitOfWork.CommentRepository.Get(c => c.Id == updateCommentDto.CommentId);
            if (comment is null) throw new BadRequestException("There isn't a comment with the provided id!");
            if (comment.UserId != user.Id) throw new NotAuthorizedException();
            comment.Content = updateCommentDto.Content;
            await _unitOfWork.Complete();
            return new CommentDto { Id = comment.Id, Content = comment.Content, CommentedBy = user, CreatedAt = comment.CreatedAt };
        }
    }
}
