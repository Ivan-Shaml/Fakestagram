using Fakestagram.Exceptions;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public FollowsController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        [HttpPut]
        public ActionResult<object> FollowUser(Guid targetUserId)
        {
            try
            {
                _userService.Follow(targetUserId);

                return Ok();
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

        [HttpDelete]
        public ActionResult<object> UnfollowUser(Guid targetUserId)
        {
            try
            {
                _userService.Unfollow(targetUserId);

                return Ok();
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
