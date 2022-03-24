using Fakestagram.Data.DTOs.Posts;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPostService _postService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSr;

        public PostsController(IUserService userService, IPostService postService,
            IJsonErrorSerializerHelper jsonErrorSr)
        {
            _userService = userService;
            _postService = postService;
            _jsonErrorSr = jsonErrorSr;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<PostReadDTO>> GetAll()
        {
            return Ok(_postService.GetAll());
        }

        [HttpGet("{postId}", Name = "GetPostById")]
        [AllowAnonymous]
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

                return CreatedAtRoute("GetPostById", new { postId = postReadDTO.PostId }, postReadDTO);
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
                if (!_userService.IsCurrentUserAdmin() && _postService.GetByIdToModel(postId)?.UserCreatorId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

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
