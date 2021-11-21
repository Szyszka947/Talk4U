using PeerToPeerCall.Data.Entities;
using System.Threading.Tasks;

namespace PeerToPeerCall.Services.User
{
    public interface ISearchUserByUniqueCredentialsService
    {
        Task<UserEntity> SearchByIdAsync(int id);
        Task<UserEntity> SearchByEmailAsync(string email);
        Task<UserEntity> SearchByUserNameAsync(string userName);
    }
}
