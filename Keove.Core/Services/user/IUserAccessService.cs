using Keove.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.Core.Services.user
{
   public interface IUserAccessService
    {
        UserDto GetCurrentUser();
    }
}
