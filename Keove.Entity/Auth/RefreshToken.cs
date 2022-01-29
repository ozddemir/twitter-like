using Keove.Entity.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Keove.Entity.Auth
{
   public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
