using Keove.Entity.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Entity.User.Follow
{
    public class Follower : BaseEntity
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
