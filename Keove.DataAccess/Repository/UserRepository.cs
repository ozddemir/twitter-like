using Keove.DataAccess.Context;
using Keove.DataAccess.IRepository;
using Keove.Dto.Configuration;
using Keove.Entity.Auth;
using Keove.Entity.User.Follow;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.Repository
{
    public class UserRepository 
    {
        //private readonly DbContext _context;
        //private readonly IMongoCollection<ApplicationUser> _collection;
        //public UserRepository(IOptions<DbSettings> settings) : base(settings)
        //{
        //    _context = new DbContext(settings);
        //    _collection = _context.GetCollection<ApplicationUser>();
        //}
        //public Task AddFollwerAsync(ObjectId key, Follower entity)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
