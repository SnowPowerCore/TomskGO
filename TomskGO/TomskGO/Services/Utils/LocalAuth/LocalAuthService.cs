using System.Threading.Tasks;
using TomskGO.Core.Services.Utils.Settings;

namespace TomskGO.Core.Services.Utils.LocalAuth
{
    public class LocalAuthService : ILocalAuthService
    {
        private readonly ISettingsService _settings;

        public LocalAuthService(ISettingsService settings)
        {
            _settings = settings;
        }

        public bool CheckAuth() =>
            _settings.GetValueOrDefault<bool>("IsSignedIn");

        public Task AuthenticateAsync() =>
            _settings.AddOrUpdateValueAsync("IsSignedIn", true);

        public Task DeauthenticateAsync() =>
            _settings.AddOrUpdateValueAsync("IsSignedIn", false);
    }
}