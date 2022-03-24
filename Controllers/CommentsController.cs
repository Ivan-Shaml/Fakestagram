using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Exceptions;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;
        private readonly IUserService _userService;

        public CommentsController(ICommentsService commentsService, IJsonErrorSerializerHelper jsonErrorSerializer, IUserService userService)
        {
            _commentsService = commentsService;
            _jsonErrorSerializer = jsonErrorSerializer;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<List<CommentReadDTO>> GetAll()
        {
            if (!_userService.IsCurrentUserAdmin())
                return Forbid();

            return Ok(_commentsService.GetAllCommentsToDto());
        }

        [HttpGet("GetAllCommentsPostedByUser/{userId}")]
        public ActionResult<List<CommentReadDTO>> GetAllCommentsPostedByUser(Guid userId)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin())
                    return Forbid();

                return Ok(_commentsService.GetCommentsByUserId(userId));
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpGet("GetAllCommentsForPost/{postId}")]
        [AllowAnonymous]
        public ActionResult<List<CommentReadDTO>> GetAllCommentsForPost(Guid postId)
        {
            try
            {
                return Ok(_commentsService.GetCommentsByPostId(postId));
            }
            catch (PostNotFoundException pnfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(pnfx));
            }
        }

        [HttpGet("{commentId}", Name = "GetCommentById")]
        [AllowAnonymous]
        public ActionResult<CommentReadDTO> GetById(Guid commentId)
        {
            try
            {
                return Ok(_commentsService.GetById(commentId));
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        [HttpPut("{commentId}")]
        public ActionResult UpdateComment(Guid commentId, CommentEditDTO commentEditDTO)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _commentsService.GetById(commentId)?.UserId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

                _commentsService.Update(commentId, commentEditDTO);

                return NoContent();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        [HttpDelete("{commentId}")]

        public ActionResult Delete(Guid commentId)
        {
            try
            {
                if (!_userService.IsCurrentUserAdmin() && _commentsService.GetById(commentId)?.UserId != _userService.GetCurrentUser()?.Id)
                    return Forbid();

                _commentsService.Delete(commentId);

                return NoContent();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        [HttpPost]
        public ActionResult<CommentReadDTO> Create(CommentCreateDTO commentCreateDTO)
        {
            try
            {
                var commentReadDto = _commentsService.Insert(commentCreateDTO);

                return CreatedAtRoute("GetCommentById", new { commentId = commentReadDto.CommentId }, commentReadDto);
            }
            catch (InvalidDataException indx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(indx));
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }

    }
}
