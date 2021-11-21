using PeerToPeerCall.Models;

namespace PeerToPeerCall.Interfaces.User
{
    public interface ISignUpUserService
    {
        ApiResponse SignUpUser(UserSignUpDto userSignUpDto);
    }
}
