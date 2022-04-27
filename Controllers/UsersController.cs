using AutoMapper;
using Fakestagram.Data.DTOs.Pagination;
using Fakestagram.Data.DTOs.Users;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Fakestagram.SwaggerExamples.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
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

        /// <summary>
        /// Get single user.
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="404">User with the specified id doesn't exist.</response>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
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

        /// <summary>
        /// Get all users from the database.
        /// </summary>
        /// <remarks>
        /// Admin privileges are required. The X-Pagination HTTP header contains metadata for the pagination.
        /// </remarks>
        /// <param name="params"></param>
        /// <response code="403">User isn't admin.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<UserReadDTO>> GetAll([FromQuery] PaginationParameters @params)
        {
            if (!_userService.IsCurrentUserAdmin())
                return Forbid();

            List<UserReadDTO> allUsers = _userService.GetAll(@params);

            return Ok(allUsers);
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <remarks>
        /// Admin privileges required to delete other users. Deletes also user's posts and comments.
        /// </remarks>
        /// <param name="userId"></param>
        /// <response code="404">User with the specified id doesn't exist. See the error message for more information.</response>
        /// <response code="403">The current user isn't admin, or tries to delete another user.</response>
        /// <response code="204">User deleted successfully.</response>
        /// <response code="400">Invalid JWT claims.</response>
        [HttpDelete("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
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

        /// <summary>
        /// Edit a user as an object.
        /// </summary>
        /// <remarks>
        /// Admin privileges required to edit other users.
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="userEditDto"></param>
        /// <response code="403">The current user isn't admin, or tries to edit another user.</response>
        /// <response code="404">User with the specified id doesn't exist.</response>
        /// <response code="204">User updated successfully.</response>
        /// <response code="409">Email or username is already in use. Check the error message for more details.</response>
        [HttpPut("EditUser/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
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
                return Conflict(_jsonErrorSerializer.Serialize(eiatx));
            }
            catch (UserNameIsAlreadyTakenException uiatx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(uiatx));
            }
        }

        /// <summary>
        /// Change user role.
        /// </summary>
        /// <remarks>Admin privileges required to change the role of a user.</remarks>
        /// <param name="userId"></param>
        /// <response code="204">Role changed successfully.</response>
        /// <response code="404">User with the specified id doesn't exist.</response>
        [HttpPut("ChangeUserRole/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
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

        /// <summary>
        /// Partially update a user object.
        /// </summary>
        /// <remarks>
        /// Admin privileges required to edit other users. Only "replace" operation is supported.
        ///
        /// **Sample request:**
        /// 
        ///             PATCH /api/Users/f89d7b44-7175-4e98-d41b-08da08f6ec52
        ///             [
        ///                 {
        ///                     "op":"replace",
        ///                     "path":"/FirstName",
        ///                     "value":"Ivan"
        ///                 },
        ///                 {
        ///                     "op":"replace",
        ///                     "path":"/LastName",
        ///                     "value":"Ivanov"
        ///                  }
        ///              ]
        /// </remarks>
        /// <param name="userId"></param>
        /// <param name="updatedUser"></param>
        /// <response code="403">The current user isn't admin, or tries to edit another user.</response>
        /// <response code="404">User with the specified id doesn't exist.</response>
        /// <response code="204">User updated successfully.</response>
        /// <response code="409">Email or username is already in use. Check the error message for more details.</response>
        /// <response code="400">Invalid operation or violating data validation constraints. Check the error message for more details.</response>
        [HttpPatch("{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        public ActionResult UpdateUser(Guid userId, [FromBody] JsonPatchDocument<UserEditDTO> updatedUser)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _userService.GetCurrentUser()?.Id != userId)
                    return Forbid();

                if (!updatedUser.Operations.TrueForAll(x => x.OperationType == OperationType.Replace))
                {
                    return BadRequest(_jsonErrorSerializer.Serialize(new Exception("Only replace operation is supported.")));
                }

                var user = _userRepository.GetById(userId);

                if (user == null)
                {
                    throw new UserNotFoundException("User with the specified id was not found.");
                }

                UserEditDTO userToEdit = _autoMapper.Map<UserEditDTO>(user);

                updatedUser.ApplyTo(userToEdit, ModelState);

                if (!TryValidateModel(userToEdit))
                {
                    var errorMessage = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(v => v.ErrorMessage)
                        .ToList();

                    return BadRequest(new { errorMessage = errorMessage });
                }

                _userService.Update(userId, userToEdit);

                return NoContent();
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
            catch (EmailIsAlreadyTakenException eiatx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(eiatx));
            }
            catch (UserNameIsAlreadyTakenException uiatx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(uiatx));
            }
        }
    }
}
