using System.Threading.Tasks;


namespace App
{
    public interface IAuthService
    {
        Task<bool> IsUserAuthenticated(string token);
    }
}