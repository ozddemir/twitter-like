using Keove.Core.Services.user;
using Keove.Dto.Responses;
using Keove.Entity.Auth;
using Keove.Entity.User.Follow;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Keove.Api.Controllers
{
    [Route("api/follow")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    public class FollowController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserAccessService _userAccessService;
        public FollowController(UserManager<ApplicationUser> userManager, IUserAccessService userAccessService)
        {
            _userManager = userManager;
            _userAccessService = userAccessService;
        }

        [HttpPut("follower/{id}")]
        public async Task<IActionResult> Create(string id)
        {

            var userToBeFollowed = await _userManager.FindByIdAsync(id);
            if(userToBeFollowed == null)
            {
                return NotFound(new ErrorResponseDto()
                {
                    Success = false,

                    Errors = new List<string>()
                        {
                            "There is no such user"
                        }
                });
            }
            var currentUser = _userAccessService.GetCurrentUser();
            if(currentUser != null)
            {

                // check if user already following
                bool flag = userToBeFollowed.Followers.Any(x => x.UserId == currentUser.Id);
                if (flag)
                {
                    return BadRequest(new ErrorResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "You are already following this user"
                        }
                    });
                }

                if (currentUser.Id == userToBeFollowed.Id.ToString())
                {

                    //refactor
                    return BadRequest(new ErrorResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "you cant follow your self"
                        }
                    });
                }

                var follower = new Follower
                {
                    Id = ObjectId.GenerateNewId(),
                    UserId = currentUser.Id,
                    UserEmail = currentUser.Email,
                    AddDate = DateTime.UtcNow,
                    UpdateDate = DateTime.UtcNow,
                    IsDeleted = false
                };

                userToBeFollowed.Followers.Add(follower);
                try
                {
                    await _userManager.UpdateAsync(userToBeFollowed);
                    return Ok(new DetailResponseDto<ApplicationUser>
                    {
                        Id = userToBeFollowed.Id.ToString(),
                        Success = true
                    });
                }
                catch (Exception ex)
                {

                    return BadRequest(new ErrorResponseDto()
                    {
                        Success = false,
                        Message = ex.Message
                    });
                }
            }

            return BadRequest(new ErrorResponseDto()
            {
                Success = false,
                Errors = new List<string>()
                        {
                            "Something went wrong"
                        }
            });
        }
    }
}