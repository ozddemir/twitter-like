using Keove.Entity.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Entity.Post
{
    public class Like : BaseEntity
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
    }
}
