namespace PeerToPeerCall.Services.JWT
{
    public interface IGetCredentialsFromAccessTokenService
    {
        (int Id, string UserName) Get(string accessToken);
    }
}
