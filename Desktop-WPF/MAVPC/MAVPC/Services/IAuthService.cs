namespace MAVPC.Services
{
    public interface IAuthService
    {
        bool Login(string username, string? password);
    }
}