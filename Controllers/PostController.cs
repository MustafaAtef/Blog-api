using BlogApi.Dtos;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.Post;
using BlogApi.Dtos.React;
using BlogApi.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers {
    [Route("api/Posts")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IReactService _reactService;

        public PostController(IPostService postService, ICommentService commentService, IReactService reactService)
        {
            _postService = postService;
            _commentService = commentService;
            _reactService = reactService;
        }

        [HttpPost]
        public async Task<ActionResult<CompletePostDto>> CreatePost(CreatePostDto createPostDto) {
            return await _postService.CreatePost(createPostDto);
        }

        [HttpGet]
        public async Task<ActionResult<PageList<CompactPostDto>>> GetAllPosts([FromQuery]SelectionOptions selectionOptions) {
            return await _postService.GetAllPosts(selectionOptions);
        }
        [HttpGet("{postId}")]
        public async Task<ActionResult<CompletePostDto>> GetPostById(int postId) {
            return await _postService.GetPostById(postId);
        }

        [HttpPost("{postId}/Comments")]
        public async Task<ActionResult<CommentDto>> CreateComment(int postId, CreateCommentDto createCommentDto) {
            createCommentDto.PostId = postId;
            return await _commentService.CreateComment(createCommentDto);
        }

        [HttpPut("{postId}/Comments/{commentId}")]
        public async Task<ActionResult<CommentDto>> UpdateComment(int commentId, UpdateCommentDto 
            updateCommentDto) {
            updateCommentDto.CommentId = commentId;
            return await _commentService.UpdateComment(updateCommentDto);
        }

        [HttpPost("{postId}/Reacts")]
        public async Task<ActionResult<ReactDto>> CreateReact(int postId, ReqReactDto reqReactDto) {
            reqReactDto.PostId = postId;
            return await _reactService.CreateReact(reqReactDto);
        }

        [HttpGet("{postId}/Reacts")]
        public async Task<ActionResult<PageList<ReactDto>>> GetAllPostReacts(int postId, [FromQuery]SelectionOptions selectionOptions) {
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
