using Gote;
using Gote.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 認証系のサービスを登録
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Supabaseサービスを登録
//builder.Services.AddScoped<ISupabaseService, MockSupabaseService>();
builder.Services.AddScoped<ISupabaseService, SupabaseService>();

// カテゴリ状態管理サービスを登録
builder.Services.AddScoped<CategoryStateService>();

await builder.Build().RunAsync();

