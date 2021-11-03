using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using RedisBloomBlazor.Data;

namespace RedisBloomBlazor
{
    public class AuthProvider :AuthenticationStateProvider
    {
        private ClaimsIdentity _identity = new ClaimsIdentity();
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(_identity)));
        }

        public void AuthorizeUser(User user)
        {
            var claims = new[] {new Claim(ClaimTypes.Name, user.Username)};
            _identity = new ClaimsIdentity(claims, "auth");
            var principle = new ClaimsPrincipal(_identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principle)));
        }
        
        public void LogOutUser()
        {
            _identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(_identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}