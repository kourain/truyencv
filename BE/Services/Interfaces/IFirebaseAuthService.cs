using TruyenCV.DTOs.Request;
using TruyenCV.Models;

namespace TruyenCV.Services
{
    public interface IFirebaseAuthService
    {
        Task<User> SignInWithFirebaseAsync(FirebaseLoginRequest request);
    }
}
