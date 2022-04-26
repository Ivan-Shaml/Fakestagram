using AutoMapper;
using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;
        private readonly IPostService _postService;
        private readonly IUsersRepository _userRepository;
        private readonly IMapper _autoMapper;

        public UsersController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer,
            IPostService postService, IUsersRepository userRepository, IMapper autoMapper)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
            _postService = postService;
            _userRepository = userRepository;
            _autoMapper = autoMapper;
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
        public ActionResult<List<UserReadDTO>> GetAll([FromQuery] PaginationParameters @params)
        {
            if (!_userService.IsCurrentUserAdmin())
                return Forbid();

            List<UserReadDTO> allUsers = _userService.GetAll(@params);

            return Ok(allUsers);
        }

        [HttpDelete("{userId}")]
        public ActionResult DeleteUser(Guid userId)
        {
            try
            {
                var user = _userService.GetById(userId);

                if (!_userService.IsCurrentUserAdmin() && userId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

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

        [HttpPut("EditUser/{userId}")]
        public ActionResult EditUser(Guid userId, UserEditDTO userEditDto)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _userService.GetCurrentUser()?.Id != userId)
                    return Forbid();

                _userService.Update(userId, userEditDto);

                return NoContent();
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EmailIsAlreadyTakenException eiatx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(eiatx));
            }
            catch (UserNameIsAlreadyTakenException uiatx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(uiatx));
            }
        }

        [HttpPut("ChangeUserRole/{userId}")]
        public ActionResult ChangeUserRole(Guid userId)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin())
                    return Forbid();

                _userService.ChangeUserRole(userId);

                return NoContent();
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpPatch("{userId}")]
        public ActionResult UpdateUser(Guid userId, [FromBody] JsonPatchDocument<UserEditDTO> updatedUser)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _userService.GetCurrentUser()?.Id != userId)
                    return Forbid();

                var user = _userRepository.GetById(userId);

                if (user == null)
                {
                    throw new UserNotFoundException("User with the specified id was not found.");
                }

                UserEditDTO userToEdit = _autoMapper.Map<UserEditDTO>(user);

                updatedUser.ApplyTo(userToEdit, ModelState);

                _userService.Update(userId, userToEdit);

                return NoContent();
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EmailIsAlreadyTakenException eiatx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(eiatx));
            }
            catch (UserNameIsAlreadyTakenException uiatx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(uiatx));
            }
        }
    }
}
