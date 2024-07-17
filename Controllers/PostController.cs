﻿using BlogApi.Dtos;
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

        public PostController(IPostService postService)
        {
            _postService = postService;
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
    }
}
