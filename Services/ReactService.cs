﻿using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.React;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Services {
    public class ReactService : IReactService {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;

        public ReactService(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
        }
        public async Task<ReactDto> CreateReact(ReqReactDto reqReactDto) {
            var post = await _appDbContext.Set<Post>().SingleOrDefaultAsync(p => p.Id == reqReactDto.PostId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var react = await _appDbContext.Set<PostReaction>().SingleOrDefaultAsync(pr => pr.PostId == reqReactDto.PostId && pr.UserId == user.Id);
            if (react is not null && react.UserId != user.Id) throw new NotAuthorizedException(); 
            else if (react is null) {
                react = new PostReaction { 
                    PostId = reqReactDto.PostId,
                    UserId = user.Id,
                    reactiontType = (ReactiontType)reqReactDto.ReactionType
                };
                _appDbContext.Set<PostReaction>().Add(react);
                post.TotalReactions++;
               
            } else if ((int)react.reactiontType != reqReactDto.ReactionType) {
                react.reactiontType = (ReactiontType)reqReactDto.ReactionType;
                react.ReactedAt = DateTime.UtcNow;
            }
            await _appDbContext.SaveChangesAsync();
            return new ReactDto { reactiontType = react.reactiontType, ReactedAt = react.ReactedAt, ReactedBy = user };
        }

        public async Task<ReactDto> DeleteReact(int postId) {
            var post = await _appDbContext.Set<Post>().SingleOrDefaultAsync(p => p.Id == postId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var react = await _appDbContext.Set<PostReaction>()
                .SingleOrDefaultAsync(pr => pr.PostId == postId && pr.UserId == user.Id);
            if (react is null) throw new BadRequestException("Invalid!");
            _appDbContext.Set<PostReaction>().Remove(react);
            post.TotalReactions--;
            await _appDbContext.SaveChangesAsync();
            return new ReactDto { reactiontType = react.reactiontType, ReactedAt = react.ReactedAt, ReactedBy = user };
        }

        public async Task<PageList<ReactDto>> GetAllPostReacts(int postId, SelectionOptions selectionOptions) {
            var post = await _appDbContext.Set<Post>().SingleOrDefaultAsync(p => p.Id == postId);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            var reacts = await _appDbContext.Set<PostReaction>().Where(pr => pr.PostId == postId)
                .Skip((selectionOptions.Page - 1) * selectionOptions.PageSize)
                .Take(selectionOptions.PageSize)
                .Select(pr => new ReactDto { 
                    reactiontType = pr.reactiontType,
                    ReactedAt = pr.ReactedAt,
                    ReactedBy = new CreatedByUserDto { 
                        Id = pr.User.Id,
                        Email = pr.User.Email,
                        Image = pr.User.Image,
                        Username = pr.User.Username
                    } 
                })
                .ToListAsync();
            return new PageList<ReactDto>(reacts, selectionOptions.Page, selectionOptions.PageSize, post.TotalReactions);
        }
    }
}
