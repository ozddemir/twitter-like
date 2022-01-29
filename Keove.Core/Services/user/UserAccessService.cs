using Keove.Dto.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Keove.Core.Services.user
{
    public class UserAccessService : IUserAccessService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAccessService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public UserDto GetCurrentUser()
        {
            if (_httpContextAccessor.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var claims = identity.Claims;
                var user = new UserDto
                {
                    Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Id = claims.FirstOrDefault(x => x.Type == "Id")?.Value, // refactor
                    Role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value

                };
                return user;
            }
            return null;
        }
    }
}
