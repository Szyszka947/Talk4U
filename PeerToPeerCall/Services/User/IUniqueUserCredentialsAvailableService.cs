namespace PeerToPeerCall.Interfaces.User
{
    public interface IUniqueUserCredentialsAvailableService
    {
        bool IsEmailAvailable(string email);
        bool IsUserNameAvailable(string userName);
    }
}
