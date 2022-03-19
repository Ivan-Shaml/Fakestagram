using Fakestagram.Data.DTOs;
using Fakestagram.Exceptions;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly ILikesService _likesService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public LikesController(ILikesService likesService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _likesService = likesService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        [HttpGet("GetPostLikesList/{postId}")]
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

        [HttpGet("GetCommentLikesList/{commentId}")]
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

        [HttpDelete("DislikeComment/{commentId}")]
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
                return BadRequest(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        [HttpDelete("DislikePost/{postId}")]
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
                return BadRequest(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }

        [HttpPut("LikeComment/{commentId}")]
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
                return BadRequest(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }
        
        [HttpPut("LikePost/{postId}")]
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
                return BadRequest(_jsonErrorSerializer.Serialize(eadx));
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
        }
    }
}
