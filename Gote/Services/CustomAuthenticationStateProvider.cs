using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Gote.Services
{
    /// <summary>
    /// 認証状態プロバイダーのカスタム実装
    /// </summary>
    internal sealed class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        /// <summary>
        /// アプリケーション コンテキスト内の現在のユーザーを表します。
        /// </summary>
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        /// <summary>
        /// 現在の認証状態を取得します。
        /// </summary>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        /// <summary>
        /// 指定されたユーザーを認証済みとしてマークします。
        /// </summary>
        public void MarkUserAsAuthenticated(Guid userId)
        {
            _currentUser = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(
            [
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userId.ToString())
            ], "CustomAuth"));
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        /// <summary>
        /// 認証状態をリセットして、現在のユーザーをログアウト済みとしてマークします。
        /// </summary>
        public void MarkUserAsLoggedOut()
        {
            _currentUser = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
