namespace MAVPC.Services
{
    public class AuthService : IAuthService
    {
        public bool Login(string username, string? password)
        {
            return !string.IsNullOrWhiteSpace(username);
        }
    }
}