using Keove.DataAccess.Context;
using Keove.DataAccess.IRepository;
using Keove.Dto.Configuration;
using Keove.Entity.General;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly DbContext _context;
        private readonly IMongoCollection<TEntity> _collection;

        public Repository(IOptions<DbSettings> settings)
        {
            _context = new DbContext(settings);
            _collection = _context.GetCollection<TEntity>();
        }

        public virtual async Task<TEntity> FindByIdAsync(ObjectId key)
        {
            var result = await _collection.FindAsync(q => q.Id == key);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            var res = await _collection.AsQueryable().ToListAsync();
            return res;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(int? offset, int? count)
        {
            var res = await _collection.Find(_ => true).Limit(count).Skip(offset).ToListAsync();
            return res;
        }

        public virtual async Task InsertOneAsync(TEntity entity)
        {

            entity.UpdateDate = DateTime.UtcNow;
            entity.AddDate = DateTime.UtcNow;
            entity.IsDeleted = false;
            await _collection.InsertOneAsync(entity);
        }


        public virtual async Task UpdateAsync(TEntity entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        }
    }
}
