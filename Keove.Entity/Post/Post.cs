using Keove.Entity.Auth;
using Keove.Entity.General;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Entity.Post
{
   public class Post : BaseEntity
    {
        public string Content { get; set; }
        public string UserId { get; set; }
        public List<Like> Likes { get; set; } = new List<Like>();
    }
}

