using PeerToPeerCall.Data.AppDbContext;
using PeerToPeerCall.Interfaces.User;
using System.Linq;

namespace PeerToPeerCall.Services.User
{
    public class UserCredentialsAvailableServiceImpl : IUniqueUserCredentialsAvailableService
    {
        private readonly AppDbContext _dbContext;

        public UserCredentialsAvailableServiceImpl(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool IsEmailAvailable(string email) => !_dbContext.Users.Any(p => p.Email == email);

        public bool IsUserNameAvailable(string userName) => !_dbContext.Users.Any(p => p.UserName == userName);
    }
}
