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
    public class FollowsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public FollowsController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        /// <summary>
        /// Follow a user.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <response code="204">User has been followed successfully.</response>
        /// <response code="404">User with the specified id doesn't exist.</response>
        /// <response code="400">Invalid JWT claims.</response>
        /// <response code="409">User already followed.</response>
        [HttpPut("{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        public ActionResult FollowUser(Guid targetUserId)
        {
            try
            {
                _userService.Follow(targetUserId);

                return NoContent();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EntityAlreadyLikedException uafe)
            {
                return Conflict(_jsonErrorSerializer.Serialize(uafe));
            }
        }

        /// <summary>
        /// Unfollow a user.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <response code="204">User has been unfollowed successfully.</response>
        /// <response code="404">User with the specified id doesn't exist.</response>
        /// <response code="400">Invalid JWT claims.</response>
        /// <response code="409">User already unfollowed.</response>
        [HttpDelete("{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        public ActionResult UnfollowUser(Guid targetUserId)
        {
            try
            {
                _userService.Unfollow(targetUserId);

                return NoContent();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EntityAlreadyDislikedException eadx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(eadx));
            }
        }

        /// <summary>
        /// Get the followers of a user.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <response code="404">User with the specified id doesn't exist.</response>
        [HttpGet("GetUserFollowers/{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<List<UserListFollowsDTO>> GetUserFollowers(Guid targetUserId)
        {
            try
            {
                var usersFollowersList = _userService.GetUserFollowers(targetUserId);

                return Ok(usersFollowersList);
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        /// <summary>
        /// Get which users, the user follows.
        /// </summary>
        /// <param name="targetUserId"></param>
        /// <response code="404">User with the specified id doesn't exist.</response>
        [HttpGet("GetUserFollowings/{targetUserId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<List<UserListFollowsDTO>> GetUserFollowings(Guid targetUserId)
        {
            try
            {
                var usersFollowingsList = _userService.GetUserFollowings(targetUserId);

                return Ok(usersFollowingsList);
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }
    }
}
