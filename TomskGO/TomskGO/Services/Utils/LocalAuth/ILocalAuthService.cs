using System.Threading.Tasks;

namespace TomskGO.Core.Services.Utils.LocalAuth
{
    public interface ILocalAuthService
    {
        bool CheckAuth();

        Task AuthenticateAsync();

        Task DeauthenticateAsync();
    }
}