using BlogApi.Dtos.React;
using BlogApi.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogApi.ServiceContracts;

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
        public async Task<ActionResult<ReactDto>> CreateUpdateReact(int postId, ReqReactDto reqReactDto) {
            reqReactDto.PostId = postId;
            return await _reactService.CreateReact(reqReactDto);
        }

        [HttpGet("{postId}/Reacts")]
        public async Task<ActionResult<PageList<ReactDto>>> GetAllPostReacts(int postId, [FromQuery] SelectionOptions selectionOptions) {
            return await _reactService.GetAllPostReacts(postId, selectionOptions);
        }

        [HttpDelete("{postId}/Reacts")]
        public async Task<ActionResult<ReactDto>> DeleteReact(int postId) {
            var deletedReact = await _reactService.DeleteReact(postId);
            if (deletedReact is null) return NotFound();
            return deletedReact;
        }
    }
}
