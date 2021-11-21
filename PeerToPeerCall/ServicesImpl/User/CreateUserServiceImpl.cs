using PeerToPeerCall.Data.AppDbContext;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Interfaces.User;

namespace PeerToPeerCall.Services.User
{
    public class CreateUserServiceImpl : ICreateUserService
    {
        private readonly AppDbContext _dbContext;

        public CreateUserServiceImpl(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateUser(UserEntity user)
        {
            _dbContext.Add(user);
            _dbContext.SaveChanges();
        }
    }
}
