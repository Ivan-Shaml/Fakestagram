using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Services.Contracts;
using Fakestagram.SwaggerExamples.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PostsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSr;

        public PostsController(IUserService userService, IPostService postService,
            IJsonErrorSerializerHelper jsonErrorSr)
        {
            _userService = userService;
            _postService = postService;
            _jsonErrorSr = jsonErrorSr;
        }

        /// <summary>
        /// Get all posts from the database.
        /// </summary>
        /// <remarks>
        /// The X-Pagination HTTP header contains metadata for the pagination.
        /// </remarks>
        /// <param name="params"></param>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<PostReadDTO>> GetAll([FromQuery] PaginationParameters @params)
        {
            return Ok(_postService.GetAll(@params));
        }

        /// <summary>
        /// Get single post.
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="404">Post with the specified id doesn't exist.</response>
        [HttpGet("{postId}", Name = "GetPostById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<PostReadDTO> GetById(Guid postId)
        {
            try
            {
                var postReadDto = _postService.GetById(postId);

                return Ok(postReadDto);
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSr.Serialize(idx));
            }
        }

        /// <summary>
        /// Create a new post.
        /// </summary>
        /// <param name="postCreateDTO"></param>
        /// <response code="201">Post created successfully.</response>
        /// <response code="400">File type not allowed.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult<PostReadDTO> CreateNewPost([FromForm] PostCreateDTO postCreateDTO)
        {
            try
            {
                //string imgPath = _postService.UploadImage(image);
                var postReadDTO = _postService.SaveNewPost(postCreateDTO);

                return CreatedAtRoute("GetPostById", new { postId = postReadDTO.PostId }, postReadDTO);
            }
            catch (InvalidDataException idx)
            {
                return BadRequest(_jsonErrorSr.Serialize(idx));
            }
        }

        /// <summary>
        /// Delete a post.
        /// </summary>
        /// <remarks>Admin privileges are required to delete another user's posts. </remarks>
        /// <param name="postId"></param>
        /// <response code="204">Post deleted successfully.</response>
        /// <response code="404">
        /// Post with the specified id doesn't exist or the file doesn't exist. 
        /// Check the error message for more information.
        /// </response>
        /// <response code="403">The current user doesn't own this post, or isn't admin.</response>
        [HttpDelete("{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult DeletePost(Guid postId)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _postService.GetByIdToModel(postId)?.UserCreatorId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

                _postService.Delete(postId);

                return NoContent();
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSr.Serialize(idx));
            }
            catch (FileNotFoundException fnfx)
            {
                return NotFound(_jsonErrorSr.Serialize(fnfx));
            }
        }

        /// <summary>
        /// Update a post.
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="postEditDto"></param>
        /// <response code="204">Post updated successfully.</response>
        /// <response code="404">Post with the specified id doesn't exist.</response>
        /// <response code="403">The current user doesn't own this post.</response>
        [HttpPut("{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<PostReadDTO> UpdatePost(Guid postId, PostEditDTO postEditDto)
        {
            try
            {
                var currentUser = _userService.GetCurrentUser();
                var post = _postService.GetByIdToModel(postId);

                if (post.UserCreatorId != currentUser.Id)
                {
                    return Forbid();
                }

                _postService.Update(postId, postEditDto);

                return NoContent();
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSr.Serialize(idx));
            }
        }
    }
}
