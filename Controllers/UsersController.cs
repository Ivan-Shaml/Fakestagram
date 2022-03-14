using Fakestagram.Data.DTOs.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController()
        {
        }


        [HttpGet]
        public ActionResult<UserReadDTO> GetById(Guid id)
        {
            return null;
        }
    }
}
