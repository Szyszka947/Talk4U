using Microsoft.EntityFrameworkCore;
using PeerToPeerCall.Data.Entities;

namespace PeerToPeerCall.Data.AppDbContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    }
}
