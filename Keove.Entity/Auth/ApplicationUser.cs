using AspNetCore.Identity.MongoDbCore.Models;
using Keove.Entity.User.Follow;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Entity.Auth
{
    [CollectionName("Users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public List<Follower> Followers { get; set; } = new List<Follower>();
    }
}
