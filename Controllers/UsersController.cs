using Fakestagram.Data.DTOs.Users;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;
        private readonly IPostService _postService;

        public UsersController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer, IPostService postService)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
            _postService = postService;
        }


        [HttpGet("{userId}")]
        public ActionResult<UserReadDTO> GetById(Guid userId)
        {
            try
            {
                var userReadDto = _userService.GetById(userId);
                return Ok(userReadDto);
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpGet]
        public ActionResult<List<UserReadDTO>> GetAll()
        {
            List<UserReadDTO> allUsers = _userService.GetAll();

            return Ok(allUsers);
        }

        [HttpDelete("{userId}")]
        public ActionResult DeleteUser(Guid userId)
        {
            try
            {
                var user = _userService.GetById(userId);

                List<Post> userPosts = _postService.GetAllByUserCreatorId(userId);

                foreach (var post in userPosts)
                {
                    _postService.Delete(post.Id);
                }

                _userService.Delete(userId);
                return NoContent();

            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(idx));
            }
            catch (UserNotFoundException unx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unx));
            }
            catch (FileNotFoundException fnfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(fnfx));
            }
        }
    }
}
