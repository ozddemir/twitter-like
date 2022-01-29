using Keove.DataAccess.Context;
using Keove.DataAccess.IRepository;
using Keove.Dto.Configuration;
using Keove.Entity.Post;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.Repository
{
   public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly DbContext _context;
        private readonly IMongoCollection<Post> _collection;
        public PostRepository(IOptions<DbSettings> settings) : base(settings)
        {
            _context = new DbContext(settings);
            _collection = _context.GetCollection<Post>();
        }

        public async Task AddLikeAsync(ObjectId id, Like entity)
        {

            entity.UpdateDate = DateTime.UtcNow;
            entity.AddDate = DateTime.UtcNow;
            entity.IsDeleted = false;
            entity.Id = ObjectId.GenerateNewId(); 
            var filter = Builders<Post>.Filter.And(
            Builders<Post>.Filter.Where(x => x.Id == id));

            var update = Builders<Post>.Update.Push("Likes", entity);

            await _collection.FindOneAndUpdateAsync(filter, update);
        }

    }
}
