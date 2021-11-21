using Microsoft.EntityFrameworkCore;
using PeerToPeerCall.Data.AppDbContext;
using PeerToPeerCall.Data.Entities;
using PeerToPeerCall.Services.User;
using System.Threading.Tasks;

namespace PeerToPeerCall.ServicesImpl.User
{
    public class SearchUserByUniqueCredentialsServiceImpl : ISearchUserByUniqueCredentialsService
    {
        private readonly AppDbContext _dbContext;


        public SearchUserByUniqueCredentialsServiceImpl(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<UserEntity> SearchByEmailAsync(string email) =>
            await _dbContext.Users.SingleOrDefaultAsync(p => p.Email == email);

        public async Task<UserEntity> SearchByIdAsync(int id) =>
            await _dbContext.Users.SingleOrDefaultAsync(p => p.Id == id);

        public async Task<UserEntity> SearchByUserNameAsync(string userName) =>
            await _dbContext.Users.SingleOrDefaultAsync(p => p.UserName == userName);
    }
}
