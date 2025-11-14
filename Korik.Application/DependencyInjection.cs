using FluentValidation;
using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            #region AutoMapper

            var licenseKey = configuration["AutoMapper:LicenseKey"];
            services.AddAutoMapper(cfg =>
            {
                cfg.LicenseKey = licenseKey;
            },
                Assembly.GetExecutingAssembly()
            );

            #endregion AutoMapper

            #region FluentValidation

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            #endregion FluentValidation

            #region Mediator

            services.AddMediatR(config =>
                   config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
            );

            #endregion Mediator

            #region Generic Service

            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

            #endregion Generic Service

            #region CarOwnerProfileService

            services.AddScoped(typeof(ICarOwnerProfileService), typeof(CarOwnerProfileService));

            #endregion CarOwnerProfileService

            #region Fluent Email

            // Configure FluentEmail with Gmail
            services
                .AddFluentEmail("ahmedmah1284@gmail.com") // sender email
                .AddSmtpSender(new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // TLS port
                    Credentials = new System.Net.NetworkCredential(
                        "ahmedmah1284@gmail.com",
                        "ekdx ewdr rzij bpfe" // your Gmail App Password
                    ),
                    EnableSsl = true
                });

            #endregion Fluent Email

            #region AuthService & AccountService

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IAccountService, AccountService>();

            #endregion AuthService & AccountService

            return services;
        }
    }
}