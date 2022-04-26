using Fakestagram.Data.DTOs;
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
    public class LikesController : ControllerBase
    {
        private readonly ILikesService _likesService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public LikesController(ILikesService likesService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _likesService = likesService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        /// <summary>
        /// Get list of users who liked the post.
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="404">Post with the specified id doesn't exist.</response>
        [HttpGet("GetPostLikesList/{postId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<List<UserListLikesDTO>> GetPostLikesList(Guid postId)
        {
            try
            {
                var userLikedList = _likesService.GetUsersLikedPostList(postId);

                return Ok(userLikedList);
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        /// <summary>
        /// Get list of users who liked the comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="404">Comment with the specified id doesn't exist.</response>
        [HttpGet("GetCommentLikesList/{commentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<List<UserListLikesDTO>> GetCommentLikesList(Guid commentId)
        {
            try
            {
                var userLikedList = _likesService.GetUsersLikedCommentList(commentId);

                return Ok(userLikedList);
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        /// <summary>
        /// Dislike a comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="204">Comment disliked successfully.</response>
        /// <response code="409">Comment already disliked.</response>
        /// <response code="404">Comment with the specified id doesn't exist.</response>
        /// <response code="400">Invalid JWT claims.</response>
        [HttpDelete("DislikeComment/{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult DislikeComment(Guid commentId)
        {
            try
            {
                _likesService.DislikeComment(commentId);
                
                return NoContent();
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EntityAlreadyDislikedException eadx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        /// <summary>
        /// Dislike a post.
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="204">Post disliked successfully.</response>
        /// <response code="409">Post already disliked.</response>
        /// <response code="404">Post with the specified id doesn't exist.</response>
        /// <response code="400">Invalid JWT claims.</response>
        [HttpDelete("DislikePost/{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult DislikePost(Guid postId)
        {
            try
            {
                _likesService.DislikePost(postId);

                return NoContent();
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EntityAlreadyDislikedException eadx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        /// <summary>
        /// Like a comment.
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="409">Comment already liked.</response>
        /// <response code="404">Comment with the specified id doesn't exist.</response>
        /// <response code="400">Invalid JWT claims.</response>
        /// <response code="204">Comment liked successfully.</response>
        [HttpPut("LikeComment/{commentId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult LikeComment(Guid commentId)
        {
            try
            {
                _likesService.LikeComment(commentId);

                return Ok();
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EntityAlreadyLikedException eadx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        /// <summary>
        /// Like a post.
        /// </summary>
        /// <param name="postId"></param>
        /// <response code="409">Post already liked.</response>
        /// <response code="404">Post with the specified id doesn't exist.</response>
        /// <response code="400">Invalid JWT claims.</response>
        /// <response code="204">Post liked successfully.</response>
        [HttpPut("LikePost/{postId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult LikePost(Guid postId)
        {
            try
            {
                _likesService.LikePost(postId);

                return Ok();
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EntityAlreadyLikedException eadx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }
    }
}
