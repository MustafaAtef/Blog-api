using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.React;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.Repositories.Interfaces;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services {
    public class ReactService : IReactService {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public ReactService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public async Task<ReactDto> CreateReact(ReqReactDto reqReactDto) {
            var post = await _unitOfWork.PostRepository.Get(p => p.Id == reqReactDto.PostId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var react = await _unitOfWork.PostReactionRepository.Get(pr => pr.PostId == reqReactDto.PostId && pr.UserId == user.Id);
            if (react is not null && react.UserId != user.Id) throw new NotAuthorizedException(); 
            else if (react is null) {
                react = new PostReaction { 
                    PostId = reqReactDto.PostId,
                    UserId = user.Id,
                    reactiontType = (ReactiontType)reqReactDto.ReactionType
                };
                _unitOfWork.PostReactionRepository.Add(react);
                post.TotalReactions++;
               
            } else if ((int)react.reactiontType != reqReactDto.ReactionType) {
                react.reactiontType = (ReactiontType)reqReactDto.ReactionType;
                react.ReactedAt = DateTime.UtcNow;
            }
            await _unitOfWork.Complete();
            return new ReactDto { reactiontType = react.reactiontType, ReactedAt = react.ReactedAt, ReactedBy = user };
        }

        public async Task<ReactDto> DeleteReact(int postId) {
            var post = await _unitOfWork.PostRepository.Get(p => p.Id == postId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var react = await _unitOfWork.PostReactionRepository.Get(pr => pr.PostId == postId && pr.UserId == user.Id);
            if (react is null) throw new BadRequestException("Invalid!");
           _unitOfWork.PostReactionRepository.Remove(react);
            post.TotalReactions--;
            await _unitOfWork.Complete();
            return new ReactDto { reactiontType = react.reactiontType, ReactedAt = react.ReactedAt, ReactedBy = user };
        }

        public async Task<PageList<ReactDto>> GetAllPostReacts(int postId, SelectionOptions selectionOptions) {
            var post = await _unitOfWork.PostRepository.Get(p => p.Id == postId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var reacts = await _unitOfWork.PostReactionRepository.GetPaged(postId, selectionOptions);
            return new PageList<ReactDto>(reacts, selectionOptions.Page, selectionOptions.PageSize, post.TotalReactions);
        }
    }
}
