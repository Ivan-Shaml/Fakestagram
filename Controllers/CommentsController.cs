using Fakestagram.Data.DTOs.Comments;
using Fakestagram.Exceptions;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public CommentsController(ICommentsService commentsService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _commentsService = commentsService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        [HttpGet]
        public ActionResult<List<CommentReadDTO>> GetAll()
        {
            return Ok(_commentsService.GetAllCommentsToDto());
        }

        [HttpGet("GetAllCommentsPostedByUser/{userId}")]
        public ActionResult<List<CommentReadDTO>> GetAllCommentsPostedByUser(Guid userId)
        {
            try
            {
                return Ok(_commentsService.GetCommentsByUserId(userId));
            }
            catch (UserNotFoundException unfx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpGet("GetAllCommentsForPost/{postId}")]
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
