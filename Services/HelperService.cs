using BlogApi.Dtos.User;
using System.Security.Claims;

namespace BlogApi.Services {
    public static class HelperService {
        public static CreatedByUserDto GetCreatedByUser(IHttpContextAccessor _httpContextAccessor) {
            return new CreatedByUserDto {
                Id = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Email = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value,
                Image = _httpContextAccessor.HttpContext.User.FindFirst("image")?.Value,
                Username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value
            };
        }
        
    }
}
