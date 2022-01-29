using Keove.Core.Services.user;
using Keove.DataAccess.IRepository;
using Keove.Dto.Post;
using Keove.Dto.Request;
using Keove.Dto.Responses;
using Keove.Entity.Auth;
using Keove.Entity.Post;
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
    [Route("api/post")]
    [ApiController]
    public class PostController : ControllerBase
    {

        private readonly IPostRepository _postRepo;
        private readonly IUserAccessService _userAccessService;


        public PostController(IPostRepository userPostRepo,
            IUserAccessService userAccessService
            )
        {
            _userAccessService = userAccessService;
            _postRepo = userPostRepo;
        }


        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User, Guest")]
        public async Task<ActionResult<IEnumerable<GetPostDto>>> List([FromQuery] PagedValueObjectDto pvod)
        {


            var posts = await _postRepo.GetAllAsync(pvod.Offset, pvod.Count);
            var user = _userAccessService.GetCurrentUser();
            
            var result = new List<GetPostDto>();
            if(user.Role == "User")
            {
                var likes = posts.Where(x => x.Likes.Any(c => c.UserId == user.Id)).ToList();
                var unlikes = posts.Where(x => x.Likes.Any(c => c.UserId != user.Id) || x.Likes.Count == 0).ToList();

                likes.ForEach(x => { result.Add(new GetPostDto { Content = x.Content }); });
                unlikes.ForEach(x => { result.Add(new GetPostDto { Content = x.Content }); });
                return result;
            }

            posts.ToList().ForEach(x => { result.Add(new GetPostDto { Content = x.Content }); });

            return result;
        }


        [HttpPost("create")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> Create([FromBody] PostCreateDto dto)
        {
            var user = _userAccessService.GetCurrentUser();
            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    var post = new Post
                    {
                        UserId = user.Id,
                        Content = dto.Content,
                    };

                    try
                    {
                        await _postRepo.InsertOneAsync(post);
                        return Ok(new DetailResponseDto<Post>
                        {
                            Id = post.Id.ToString(),
                            Success = true
                        });
                    }
                    catch (Exception ex)
                    {

                        return BadRequest(new ErrorResponseDto()
                        {
                            Success = false,
                            Message = ex.Message,
                            Errors = new List<string>()
                        {
                            "Something went wrong"
                        }
                        });
                    }
                }
            }
            return NotFound(new ErrorResponseDto()
            {
                Success = false,

                Errors = new List<string>()
                        {
                            "There is no such user"
                        }
            });
        }

        [HttpPut("like/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        public async Task<IActionResult> Like(string id)
        {
            var objectId = ObjectId.Parse(id);
            var post = await _postRepo.FindByIdAsync(objectId);
            var user = _userAccessService.GetCurrentUser();
            if (post != null && user != null)
            {

                // check if user already liked the post
                bool flag = post.Likes.Any(x => x.UserId == user.Id);
                if (flag)
                {
                    return BadRequest(new ErrorResponseDto()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "The post is already liked by user"
                        }
                    });
                }

                var like = new Like
                {
                    UserEmail = user.Email,
                    UserId = user.Id,
                };

                try
                {
                    await _postRepo.AddLikeAsync(post.Id, like);
                    return Ok(new DetailResponseDto<Post>
                    {
                        Id = post.Id.ToString(),
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

            return NotFound(new ErrorResponseDto()
            {
                Success = false,

                Errors = new List<string>()
                        {
                            "There is no such post"
                        }
            });
        }
    }
}
