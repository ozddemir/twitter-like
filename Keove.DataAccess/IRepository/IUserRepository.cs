using Keove.Entity.Auth;
using Keove.Entity.User.Follow;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.IRepository
{
    interface IUserRepository : IRepository<ApplicationUser>
    {
        Task AddFollwerAsync(ObjectId key, Follower entity);
    }
}
