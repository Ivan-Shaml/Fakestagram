using Fakestagram.Data.DTOs.Users;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public AuthController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        [HttpPost("Register")]
        public ActionResult<string> Register(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                string jwt = _userService.RegisterNewUser(userRegisterDTO, out User newUser);

                string userHomePageUrl = Url.Action("GetById", "UsersController");

                return CreatedAtRoute(userHomePageUrl, new { Id = newUser.Id }, jwt);
            }
            catch (EmailIsAlreadyTakenException emx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(emx));
            }
            catch (UserNameIsAlreadyTakenException unx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unx));
            }
        }

        [HttpPost("Login")]
        public ActionResult<string> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                var jwt = _userService.UserLogin(userLoginDTO);

                return Ok(jwt);
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }

    }
}
