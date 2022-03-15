using Fakestagram.Data.DTOs.Users;
using Fakestagram.Exceptions;
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

        public UsersController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }


        [HttpGet("{id}")]
        public ActionResult<UserReadDTO> GetById(Guid id)
        {
            try
            {
                var userReadDto = _userService.GetById(id);
                return Ok(userReadDto);
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }
    }
}
