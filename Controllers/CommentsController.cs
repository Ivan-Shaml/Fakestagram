using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Exceptions;
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
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;
        private readonly IUserService _userService;

        public CommentsController(ICommentsService commentsService, IJsonErrorSerializerHelper jsonErrorSerializer, IUserService userService)
        {
            _commentsService = commentsService;
            _jsonErrorSerializer = jsonErrorSerializer;
            _userService = userService;
        }

        /// <summary>
        /// Get all comments in the database.
        /// </summary>
        /// <remarks>
        /// Admin privileges required.
        /// The X-Pagination HTTP header contains metadata for the pagination.
        /// </remarks>
        /// <param name="params"></param>
        /// <response code="403">Insufficient privileges.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<CommentReadDTO>> GetAll([FromQuery] PaginationParameters @params)
        {
            if (!_userService.IsCurrentUserAdmin())
                return Forbid();

            return Ok(_commentsService.GetAllCommentsToDto(@params));
        }

        /// <summary>
        /// Get all comments posted by a particular user.
        /// </summary>
        /// <remarks>
        /// Admin privileges required.
        /// The X-Pagination HTTP header contains metadata for the pagination.
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="params"></param>
        /// <response code="404">User with the specified id doesn't exist.</response>
        /// <response code="403">Insufficient privileges.</response>
        [HttpGet("GetAllCommentsPostedByUser/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<List<CommentReadDTO>> GetAllCommentsPostedByUser(Guid userId, [FromQuery] PaginationParameters @params)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin())
                    return Forbid();

                return Ok(_commentsService.GetCommentsByUserId(userId, @params));
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        /// <summary>
        /// Get all comments for a particular post.
        /// </summary>
        /// <remarks>
        /// The X-Pagination HTTP header contains metadata for the pagination.
        /// </remarks>
        /// <param name="postId"></param>
        /// <param name="params"></param>
        /// <response code="404">Post with the specified id doesn't exist.</response>
        [HttpGet("GetAllCommentsForPost/{postId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<List<CommentReadDTO>> GetAllCommentsForPost(Guid postId, [FromQuery] PaginationParameters @params)
        {
            try
            {
                return Ok(_commentsService.GetCommentsByPostId(postId, @params));
            }
            catch (PostNotFoundException pnfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(pnfx));
            }
        }

        /// <summary>
        /// Get a comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="404">Comment with the specified id doesn't exist.</response>
        [HttpGet("{commentId}", Name = "GetCommentById")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<CommentReadDTO> GetById(Guid commentId)
        {
            try
            {
                return Ok(_commentsService.GetById(commentId));
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        /// <summary>
        /// Update a comment.
        /// </summary>
        /// <remarks>Admin privileges are required to edit another user's comment.</remarks>
        /// <param name="commentId"></param>
        /// <param name="commentEditDTO"></param>
        /// <response code="204">Changes saved successfully.</response>
        /// <response code="404">Comment with the specified id doesn't exist.</response>
        /// <response code="403">Comment doesn't belong to the user, or user isn't admin.</response>
        [HttpPut("{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult UpdateComment(Guid commentId, CommentEditDTO commentEditDTO)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _commentsService.GetById(commentId)?.UserId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

                _commentsService.Update(commentId, commentEditDTO);

                return NoContent();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        /// <summary>
        /// Delete a comment.
        /// </summary>
        /// <remarks>Admin privileges are required to delete another user's comment.</remarks>
        /// <param name="commentId"></param>
        /// <response code="204">Comment deleted successfully.</response>
        /// <response code="404">Comment with the specified id doesn't exist.</response>
        /// <response code="403">Comment doesn't belong to the user, or user isn't admin.</response>
        [HttpDelete("{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult Delete(Guid commentId)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _commentsService.GetById(commentId)?.UserId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

                _commentsService.Delete(commentId);

                return NoContent();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        /// <summary>
        /// Post a new comment.
        /// </summary>
        /// <param name="commentCreateDTO"></param>
        /// <response code="201">Comment posted successfully.</response>
        /// <response code="400">Check the error message for more information.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult<CommentReadDTO> Create(CommentCreateDTO commentCreateDTO)
        {
            try
            {
                var commentReadDto = _commentsService.Insert(commentCreateDTO);

                return CreatedAtRoute("GetCommentById", new { commentId = commentReadDto.CommentId }, commentReadDto);
            }
            catch (InvalidDataException indx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(indx));
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }

    }
}
