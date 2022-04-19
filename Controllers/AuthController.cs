using Fakestagram.Data.DTOs.Users;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Fakestagram.Data.DTOs.Tokens;

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
        public ActionResult<TokenAuthDTO> Register(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                var authDto = _userService.RegisterNewUser(userRegisterDTO, out User newUser);

                string userHomePageUrl = Url.Action("GetById", "UsersController");

                return CreatedAtRoute(userHomePageUrl, new { Id = newUser.Id }, authDto);
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
        public ActionResult<TokenAuthDTO> Login(UserLoginDTO userLoginDTO)
        {
            try
            {
                var authDto = _userService.UserLogin(userLoginDTO);

                return Ok(authDto);
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpPost("Refresh")]
        public ActionResult<string> Refresh(TokenAuthDTO authDto)
        {
            try
            {
                var newJwtAuthDto = _userService.RefreshToken(authDto);

                return Ok(newJwtAuthDto);
            }
            catch (Exception ex)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(ex));
            }
        }

    }
}
