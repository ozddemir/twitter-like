using Keove.Entity.Post;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.IRepository
{
    public interface IPostRepository : IRepository<Post>
    {
        Task AddLikeAsync(ObjectId key , Like entity);
    }
}
