﻿using Fakestagram.Data.DTOs;
using Fakestagram.Exceptions;
using Fakestagram.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fakestagram.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJsonErrorSerializerHelper _jsonErrorSerializer;

        public FollowsController(IUserService userService, IJsonErrorSerializerHelper jsonErrorSerializer)
        {
            _userService = userService;
            _jsonErrorSerializer = jsonErrorSerializer;
        }

        [HttpPut("{targetUserId}")]
        public ActionResult FollowUser(Guid targetUserId)
        {
            try
            {
                _userService.Follow(targetUserId);

                return Ok();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpDelete("{targetUserId}")]
        public ActionResult UnfollowUser(Guid targetUserId)
        {
            try
            {
                _userService.Unfollow(targetUserId);

                return Ok();
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
            catch (UserNotFoundException unfx)
            {
                return BadRequest(_jsonErrorSerializer.Serialize(unfx));
            }
        }

        [HttpGet("GetUserFollowers/{targetUserId}")]

        public ActionResult<UserListFollowsDTO> GetUserFollowers(Guid targetUserId)
        {
            try
            {
                var usersFollowersList = _userService.GetUserFollowers(targetUserId);

                return Ok(usersFollowersList);
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }

        [HttpGet("GetUserFollowings/{targetUserId}")]

        public ActionResult<UserListFollowsDTO> GetUserFollowings(Guid targetUserId)
        {
            try
            {
                var usersFollowingsList = _userService.GetUserFollowings(targetUserId);

                return Ok(usersFollowingsList);
            }
            catch (InvalidDataException indx)
            {
                return NotFound(_jsonErrorSerializer.Serialize(indx));
            }
        }
    }
}