using Keove.DataAccess.Context;
using Keove.DataAccess.IRepository;
using Keove.Dto.Configuration;
using Keove.Entity.Auth;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.Repository
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly DbContext _context;
        private readonly IMongoCollection<RefreshToken> _collection;
        public RefreshTokenRepository(IOptions<DbSettings> settings) : base(settings)
        {
            _context = new DbContext(settings);
            _collection = _context.GetCollection<RefreshToken>();
        }
      
        public async Task UpsertRefreshTokenAsync(RefreshToken entity)
        {
            entity.UpdateDate = DateTime.UtcNow;
            entity.IsDeleted = false;

            var refreshTokenCollection =  await _collection.FindAsync(x => x.UserId == entity.UserId).Result.FirstOrDefaultAsync();

            // make sure every user has one refresh token
            if (refreshTokenCollection == null)
            {
                entity.Id = ObjectId.GenerateNewId();
                entity.AddDate = DateTime.UtcNow;
                await _collection.InsertOneAsync(entity);
            }
            else
            {
                entity.Id = refreshTokenCollection.Id;
                await _collection.ReplaceOneAsync(x => x.Id == refreshTokenCollection.Id, entity);
            }
        }

        public async Task<RefreshToken> FindByRefreshTokenAsync(string key)
        {
            var refreshTokenCollection = await _collection.FindAsync(x => x.Token == key).Result.FirstOrDefaultAsync();

            return refreshTokenCollection;
        }
    }
}

