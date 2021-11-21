using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PeerToPeerCall.Data;
using PeerToPeerCall.Data.AppDbContext;
using PeerToPeerCall.Hubs;
using PeerToPeerCall.Interfaces.User;
using PeerToPeerCall.Models;
using PeerToPeerCall.Services.JWT;
using PeerToPeerCall.Services.User;
using PeerToPeerCall.ServicesImpl.JWT;
using PeerToPeerCall.ServicesImpl.User;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace PeerToPeerCall
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "Talk4U/dist";
            });

            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RequireExpirationTime = true,
                        ValidAudience = JWTCfg.ValidAudience,
                        ValidIssuer = JWTCfg.ValidIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWTCfg.SecretKey)),
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            services.AddDbContext<AppDbContext>(cfg =>
            {
                cfg.UseSqlServer(_config.GetConnectionString("Default"));
            });

            services.PostConfigure<ApiBehaviorOptions>(cfg =>
            {
                cfg.InvalidModelStateResponseFactory = actionContext =>
                {
                    return new BadRequestObjectResult(new ApiResponse
                    {
                        Status = ResponseTypes.Fail,
                        Message = "One or more validation errors occurred",
                        Data = actionContext.ModelState.Keys.ToList().
                            Zip(actionContext.ModelState.Values.Select(p => p.Errors.Select(x => x.ErrorMessage).ToList()),
                                (key, errors) => new { key, errors }).ToDictionary(k => k.key, e => e.errors)
                    });
                };
            });
            // transient services
            services.AddTransient<ICreateUserService, CreateUserServiceImpl>();
            services.AddTransient<ISignUpUserService, SignUpUserServiceImpl>();
            services.AddTransient<ISignInUserService, SignInUserServiceImpl>();
            services.AddTransient<IValidateJWTService, ValidateJWTServiceImpl>();
            services.AddTransient<IAssignRefreshTokenService, AssignRefreshTokenServiceImpl>();

            // scoped services
            services.AddScoped<IUniqueUserCredentialsAvailableService, UserCredentialsAvailableServiceImpl>();
            services.AddScoped<ISearchUserByUniqueCredentialsService, SearchUserByUniqueCredentialsServiceImpl>();
            services.AddScoped<IGenerateJWTService, GenerateJWTServiceImpl>();
            services.AddScoped<ISearchRefreshTokenService, SearchRefreshTokenServiceImpl>();
            services.AddScoped<IGetCredentialsFromAccessTokenService, GetCredentialsFromAccessTokenServiceImpl>();

            // singleton services
            services.AddSingleton<SignalingHubConnectedClients>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAntiforgery antiforgery)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.Use(async (context, next) =>
            {
                var accessToken = context.Request.Cookies["accessToken"];

                context.Request.Headers["Authorization"] = "Bearer " + accessToken;

                await next();
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async(context, next) =>
            {
                if (context.Request.Path.Value.IndexOf("/api", StringComparison.OrdinalIgnoreCase) != -1 || context.Request.Method == "GET")
                {
                    var tokens = antiforgery.GetAndStoreTokens(context);

                    context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken,
                        new CookieOptions() { HttpOnly = false, SameSite = SameSiteMode.Strict });
                }

                await next();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalingHub>("/signalingHub");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Talk4U";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
