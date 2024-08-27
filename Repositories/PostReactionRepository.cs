using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.React;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogApi.Repositories {
    public class PostReactionRepository : Repository<PostReaction>, IPostReactionRepository {
        public PostReactionRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            
        }

        public async Task<List<ReactDto>> GetPaged(int postId,SelectionOptions selectionOptions) {
            return await _appDbContext.Set<PostReaction>().Where(pr => pr.PostId == postId)
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
        }
    }
}
