using MechanciShop.Infrustructure.BackgroundJobs;
using MechanciShop.Infrustructure.Data;
using MechanciShop.Infrustructure.Identity;
using MechanciShop.Infrustructure.Identity.Policies;
using MechanciShop.Infrustructure.RealTiIme;
using MechanciShop.Infrustructure.Services;
using MechanicShop.Application.Common.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace MechanciShop.Infrustructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrustructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(TimeProvider.System);

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            ArgumentNullException.ThrowIfNull(connectionString);

            services.AddDbContext<AppDbContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetService<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

            services.AddScoped<ApplicationDbContextInitialiser>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddIdentityCore<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddScoped<IAuthorizationHandler, LaborAssignedHandler>();

            services.AddAuthorizationBuilder()
              .AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"))
              .AddPolicy("SelfScopedWorkOrderAccess", policy =>
                policy.Requirements.Add(new LaborAssignedRequirement()));

            services.AddTransient<IIdentityService, IdentityService>();

            services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10), // L2, L3
                LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1
            });

            services.AddScoped<IWorkOrderPolicy, WorkOrderPolicy>();

            services.AddScoped<ITokenProvider, TokenProvider>();

            services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();

            services.AddScoped<IWorkOrderNotifier, SignalRWorkOrderNotifier>();

            services.AddHostedService<OverdueBookingCleanupService>();

            return services;
        }
    }
}
