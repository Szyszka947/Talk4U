using PeerToPeerCall.Models;
using System.Threading.Tasks;

namespace PeerToPeerCall.Services.User
{
    public interface ISignInUserService
    {
        Task<ApiResponse> SignInUserAsync(UserSignInDto userSignInDto);
    }
}
