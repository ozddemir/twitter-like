using Keove.Entity.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keove.DataAccess.IRepository
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task UpsertRefreshTokenAsync(RefreshToken entity);
        Task<RefreshToken> FindByRefreshTokenAsync(string key);

    }
}
