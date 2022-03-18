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
        private readonly IConfiguration _configuration;

        public PostsController(IUserService userService, IPostService postService,
            IJsonErrorSerializerHelper jsonErrorSr, IConfiguration configuration)
        {
            _userService = userService;
            _postService = postService;
            _jsonErrorSr = jsonErrorSr;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<List<PostReadDTO>> GetAll()
        {
            return Ok(_postService.GetAll());
        }

        [HttpGet("{postId}", Name = "GetPostById")]
        public ActionResult<PostReadDTO> GetById(Guid postId)
        {
            try
            {
                var postReadDto = _postService.GetById(postId);

                return Ok(postReadDto);
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSr.Serialize(idx));
            }
        }

        [HttpPost]
        public ActionResult<PostReadDTO> CreateNewPost([FromForm] PostCreateDTO postCreateDTO)
        {
            try
            {
                //string imgPath = _postService.UploadImage(image);
                var postReadDTO = _postService.SaveNewPost(postCreateDTO);

                return CreatedAtRoute("GetPostById", new {postId = postReadDTO.PostId}, postReadDTO);
            }
            catch (InvalidDataException idx)
            {
                return BadRequest(_jsonErrorSr.Serialize(idx));
            }
        }

        [HttpDelete("{postId}")]
        public ActionResult DeletePost(Guid postId)
        {
            try
            {
                _postService.Delete(postId);
                return NoContent();
            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSr.Serialize(idx));
            }
            catch (FileNotFoundException fnfx)
            {
                return NotFound(_jsonErrorSr.Serialize(fnfx));
            }
        }

        [HttpPut("{postId}")]
        public ActionResult<PostReadDTO> UpdatePost(Guid postId, PostEditDTO postEditDto)
        {
            try
            {
                var currentUser = _userService.GetCurrentUser();
                var post = _postService.GetByIdToModel(postId);

                if (post.UserCreatorId != currentUser.Id)
                {
                    return Forbid();
                }

                _postService.Update(postId, postEditDto);

                return NoContent();

            }
            catch (InvalidDataException idx)
            {
                return NotFound(_jsonErrorSr.Serialize(idx));
            }
        }
    }
}
