using BlogApi.Dtos.Comment;
using BlogApi.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers {
    [Route("api/Posts")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("{postId}/Comments")]

        public async Task<ActionResult<CommentDto>> CreateComment(int postId, CreateCommentDto createCommentDto) {
            createCommentDto.PostId = postId;
            return await _commentService.CreateComment(createCommentDto);
        }

        [HttpPut("Comments/{commentId}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int commentId, UpdateCommentDto
            updateCommentDto) {
            updateCommentDto.CommentId = commentId;
            return await _commentService.UpdateComment(updateCommentDto);
        }

        [HttpDelete("Comments/{commentId}")]
        public async Task<ActionResult<CommentDto>> DeleteComment(int commentId) {
            return await _commentService.DeleteComment(commentId);
        }

    }
}
