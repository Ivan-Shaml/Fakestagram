using Fakestagram.Data.DTOs.Users;
using Fakestagram.Exceptions;
using Fakestagram.Models;
using Fakestagram.SwaggerExamples.Responses;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Fakestagram.Data.DTOs.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public AuthController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="userRegisterDTO"></param>
        /// <response code="201">User successfully created. Returns new access and refresh token pair.</response>
        /// <response code="409">Username or Email already in use. More information in the error message.</response>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(CustomExceptionExample))]
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
                return Conflict(_jsonErrorSerializer.Serialize(emx));
            }
            catch (UserNameIsAlreadyTakenException unx)
            {
                return Conflict(_jsonErrorSerializer.Serialize(unx));
            }
        }

        /// <summary>
        /// User login.
        /// </summary>
        /// <param name="userLoginDTO"></param>
        /// <response code="200">User login successful. Returns new access and refresh token pair.</response>
        /// <response code="400">Wrong username or password.</response>
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
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

        /// <summary>
        /// Refresh access token.
        /// </summary>
        /// <param name="authDto"></param>
        /// <response code="200">Token refreshed. Returns new access and refresh token pair.</response>
        /// <response code="400">The provided token is invalid. More information in the error message.</response>
        /// <response code="404">The provided refresh token doesn't exist.</response>
        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult<TokenAuthDTO> Refresh(TokenAuthDTO authDto)
        {
            try
            {
                var newJwtAuthDto = _userService.RefreshToken(authDto);

                return Ok(newJwtAuthDto);
            }
            catch (InvalidRefreshTokenException irtex)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(irtex));
            }
            catch (RefreshTokenNotFoundException rtnfex)
            {
                return NotFound(_jsonErrorSerializer.Serialize(rtnfex));
            }
        }

        /// <summary>
        /// Revoke refresh token.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <response code="204">Refresh token revoked successfully.</response>
        /// <response code="400">Provided refresh token is invalid. More information in the error message.</response>
        /// <response code="404">Refresh token doesn't exist.</response>
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(CustomExceptionExample))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(CustomExceptionExample))]
        public ActionResult RevokeRefreshToken(string refreshToken)
        {
            try
            {
                _userService.RevokeRefreshToken(refreshToken);

                return NoContent();
            }
            catch (InvalidRefreshTokenException irtex)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(irtex));
            }
            catch (RefreshTokenNotFoundException rtnfex)
            {
                return NotFound(_jsonErrorSerializer.Serialize(rtnfex));
            }
        }

    }
}
