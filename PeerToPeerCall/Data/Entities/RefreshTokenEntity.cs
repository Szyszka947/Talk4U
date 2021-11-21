using System;

namespace PeerToPeerCall.Data.Entities
{
    public class RefreshTokenEntity
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public UserEntity User { get; set; }
    }
}
