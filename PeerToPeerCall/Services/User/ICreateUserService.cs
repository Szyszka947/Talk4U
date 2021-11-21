using PeerToPeerCall.Data.Entities;

namespace PeerToPeerCall.Interfaces.User
{
    public interface ICreateUserService
    {
        void CreateUser(UserEntity user);
    }
}
