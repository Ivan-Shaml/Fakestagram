using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSr;

        public PostsController(IUserService userService, IPostService postService, IJsonErrorSerializerHelper jsonErrorSr)
        {
            _userService = userService;
            _postService = postService;
            _jsonErrorSr = jsonErrorSr;
        }

        [HttpPost]
        public ActionResult<PostReadDTO> CreateNewPost([FromForm(Name = "image")] IFormFile file)
        {
            try
            {
                string imgPath = _postService.UploadImage(file);
                var postReadDTO = _postService.SaveNewPost(imgPath);

                return postReadDTO;
            }
            catch (InvalidDataException idx)
            {
                return BadRequest(_jsonErrorSr.Serialize(idx));
            }
        }
    }
}
