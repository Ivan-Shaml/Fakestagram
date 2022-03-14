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

        public AuthController(IUserService userService)
        {
            _userService = userService;
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
                return BadRequest(JsonSerializer.Serialize(new { errorMessage = emx.Message }));
            }
            catch (UserNameIsAlreadyTakenException unx)
            {
                return BadRequest(JsonSerializer.Serialize(new { errorMessage = unx.Message }));
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
                return BadRequest(JsonSerializer.Serialize(new { errorMessage = unfx.Message }));
            }
        }

    }
}
