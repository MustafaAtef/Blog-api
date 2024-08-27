using BlogApi.Database;
using BlogApi.Dtos;
using BlogApi.Dtos.Comment;
using BlogApi.Dtos.Post;
using BlogApi.Dtos.Tag;
using BlogApi.Dtos.User;
using BlogApi.Entities;
using BlogApi.Exceptions;
using BlogApi.Repositories.Interfaces;
using BlogApi.ServiceContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlogApi.Services {
    public class PostService : IPostService {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CompletePostDto> CreatePost(CreatePostDto createPostDto) {
            Category? category = await _checkCategory(createPostDto.CategoryId);
            if (category is null) throw new BadRequestException("There isn't a category with the provided id!");
            var tags = await _unitOfWork.TagRepository.Get(createPostDto.Tags);
            if (tags.Count != createPostDto.Tags.Count)
                throw new BadRequestException("There aren't tags with the provided ids");
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var post = new Post() {
                Title = createPostDto.Title,
                Content = createPostDto.Content,
                CategoryId = category.Id,
                CreatedBy = user.Id,
                PostTags = new List<PostTag>()
            };
            foreach (var tagId in createPostDto.Tags) {
                post.PostTags.Add(new PostTag { Post = post, TagId = tagId });
            }
            _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.Complete();
            return new CompletePostDto { Tags = tags, Title = post.Title, Id = post.Id, CategoryId = post.CategoryId, CategoryName = category.Name, Content = post.Content, CreatedAt = post.CreatedAt, LastModification = post.LastModification, CreatedBy = user, TotalComments = post.TotalComments, TotalReactions = post.TotalReactions, Comments = new List<CommentDto>() };
        }


        public async Task<PageList<CompactPostDto>> GetAllPosts(SelectionOptions selectionOptions) {
            var result = await _unitOfWork.PostRepository.GetPaged(selectionOptions);
            return new PageList<CompactPostDto>(result.compactPosts, selectionOptions.Page, selectionOptions.PageSize, result.TotalCount);
        }

        public async Task<CompletePostDto> GetPostById(int id) {
            var res = await _unitOfWork.PostRepository.GetComplete(id);
            if (res is null) throw new BadRequestException("There isn't a post with the provided id!");
            return res;
        }

        public async Task<CompletePostDto> UpdatePost(UpdatePostDto updatePostDto) {
            var user = HelperService.GetCreatedByUser(_httpContextAccessor);
            var post = await _unitOfWork.PostRepository.GetWithComments(p => p.Id == updatePostDto.Id);
            if (post is null) throw new BadRequestException("There isn't a post with the provided id!");
            if (post.CreatedBy != user.Id) throw new NotAuthorizedException();
            var category = await _checkCategory(updatePostDto.CategoryId);
            if (category is null) throw new BadRequestException("There isn't a category with the provided id!");
            var tags = await _unitOfWork.TagRepository.Get(updatePostDto.Tags);
            if (tags.Count != updatePostDto.Tags.Count) 
                throw new BadRequestException("There aren't tags with the provided ids");


            await _unitOfWork.TagRepository.BulkDelete(pt => pt.PostId == updatePostDto.Id);
            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            post.Category = category;
            post.LastModification = DateTime.UtcNow;
            post.PostTags = new List<PostTag>();
            foreach (var tagId in updatePostDto.Tags) {
                post.PostTags.Add(new PostTag { Post = post, TagId = tagId });
            }
            await _unitOfWork.Complete();

            return new CompletePostDto { Tags = tags, Title = post.Title, Id = post.Id, CategoryId = post.CategoryId, CategoryName = category.Name, Content = post.Content, CreatedAt = post.CreatedAt, LastModification = post.LastModification, CreatedBy = user, TotalComments = post.TotalComments, TotalReactions = post.TotalReactions,
              Comments = post.Comments
                .Select(comment => new CommentDto {
                    Id = comment.Id,
                    Content = comment.Content,
                    CreatedAt = comment.CreatedAt,
                    CommentedBy = new CreatedByUserDto {
                        Id = comment.User.Id,
                        Email = comment.User.Email,
                        Image = comment.User.Image,
                        Username = comment.User.Username
                    }
                })
            };
        }

        private Task<Category?> _checkCategory(int? categoryId) {
            if (categoryId is null) return null;
            return _unitOfWork.CategoryRepository.Get(category => category.Id == categoryId);
        }

    }
}
