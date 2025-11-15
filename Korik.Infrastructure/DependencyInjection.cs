using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            #region AddDbContext

            services.AddDbContext<Korik>(options =>
                               options.UseSqlServer(configuration.GetConnectionString("Korik")));

            #endregion AddDbContext

            #region Generic Repository

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            #endregion Generic Repository

            #region CarOwnerProfileRepository

            services.AddScoped<ICarOwnerProfileRepository, CarOwnerProfileRepository>();      
                
           #endregion CarOwnerProfileRepository

            #region Car Repository
            services.AddScoped<ICarRepository, CarRepository>();
            #endregion

            #region CarOwnerProfileRepository
            services.AddScoped<ICarOwnerProfileRepository, CarOwnerProfileRepository>();
            #endregion

            #region Car Expense Repository
            services.AddScoped<ICarExpenseRepository, CarExpenseRepository>();
            #endregion


            #region Identity Services

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // -------------------
                // Password settings
                // -------------------
                options.Password.RequireDigit = true;                // Must contain a number
                options.Password.RequireLowercase = true;            // Must contain a lowercase letter
                options.Password.RequireUppercase = true;            // Must contain an uppercase letter
                options.Password.RequireNonAlphanumeric = true;     // Must contain a special character
                options.Password.RequiredLength = 6;                // Minimum length
                options.Password.RequiredUniqueChars = 1;           // Minimum unique characters
                options.SignIn.RequireConfirmedEmail = true;        // Optional: require email confirmation

                // -------------------
                // User settings
                // -------------------
                options.User.RequireUniqueEmail = true;           // Email must be unique
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; // Allowed username chars

                // -------------------
                // Lockout settings
                // -------------------
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout duration
                options.Lockout.MaxFailedAccessAttempts = 5;                       // Max failed attempts
                options.Lockout.AllowedForNewUsers = true;                         // Lockout enabled for new users

                // -------------------
                // SignIn settings
                // -------------------
                options.SignIn.RequireConfirmedEmail = false;     // Require email confirmation
                options.SignIn.RequireConfirmedPhoneNumber = false; // Require phone confirmation
            })
            .AddEntityFrameworkStores<Korik>()
            .AddDefaultTokenProviders(); // <-- THIS IS IMPORTANT

            #endregion Identity Services

            #region Auth External Services

            services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            #endregion Auth External Services

            return services;
        }
    }
}