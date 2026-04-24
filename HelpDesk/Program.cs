using HelpDesk.Data;
using HelpDesk.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace HelpDesk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<HelpDeskDbContext>(options =>
                options.UseSqlite("Data Source=helpdesk.db"));

            // Redis
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
                options.InstanceName = "HelpDesk_";
            });

            // Service Redis
            builder.Services.AddScoped<TicketService>();

            // KeyCloak
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
             {
                 options.AccessDeniedPath = "/Home/AccessDenied";
             })
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:8080/realms/helpdesk";
                options.ClientId = "helpdesk-web";
                options.ClientSecret = "eRiJ7ZHM7ujmcUJtnY3p2PEPnIvyLTWA";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.RequireHttpsMetadata = false;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = context =>
                    {
                        var identity = context.Principal?.Identity as ClaimsIdentity;
                        if (identity == null) return Task.CompletedTask;

                        var realAccess = context.Principal?.FindFirst("realm_access");

                        if (realAccess != null)
                        {
                            var json = System.Text.Json.JsonDocument.Parse(realAccess.Value);
                            if (json.RootElement.TryGetProperty("roles", out var roles))
                            {
                                foreach (var role in roles.EnumerateArray())
                                {
                                    identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()!));
                                }
                            }
                        }
                        return Task.CompletedTask;
                    },

                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        var logoutUri = $"{context.Options.Authority}/protocol/openid-connect/logout";
                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            logoutUri += $"?post_logout_redirect_uri={Uri.EscapeDataString(postLogoutUri)}"
                                         + $"&client_id={context.Options.ClientId}";
                        }

                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };                
            });            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
