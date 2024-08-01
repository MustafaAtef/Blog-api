using BlogApi.Dtos.React;
using BlogApi.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogApi.ServiceContracts;
using Microsoft.AspNetCore.Authorization;

namespace BlogApi.Controllers {
    [Route("api/Posts")]
    [ApiController]
    public class ReactController : ControllerBase {
        private readonly IReactService _reactService;

        public ReactController(IReactService reactService)
        {
            _reactService = reactService;
        }

        [HttpPost("{postId}/Reacts")]
        [Authorize]
        public async Task<ActionResult<ReactDto>> CreateUpdateReact(int postId, ReqReactDto reqReactDto) {
            reqReactDto.PostId = postId;
            return await _reactService.CreateReact(reqReactDto);
        }

        [HttpGet("{postId}/Reacts")]
        public async Task<ActionResult<PageList<ReactDto>>> GetAllPostReacts(int postId, [FromQuery] SelectionOptions selectionOptions) {
            return await _reactService.GetAllPostReacts(postId, selectionOptions);
        }

        [HttpDelete("{postId}/Reacts")]
        [Authorize]
        public async Task<ActionResult<ReactDto>> DeleteReact(int postId) {
           return await _reactService.DeleteReact(postId);
        }
    }
}
