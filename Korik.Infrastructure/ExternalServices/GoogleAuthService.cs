using AutoMapper;
using FluentEmail.Core;
using FluentValidation;
using Google.Apis.Auth;
using Korik.Application;
using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Infrastructure
{
    public class GoogleAuthService : IGoogleAuthService
    {
        #region Dependency Injection
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IValidator<GoogleLoginDTO> _googleLoginValidator;
        private readonly ICarOwnerProfileService _carOwnerProfileService;

        public GoogleAuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IAuthService authService,
            IConfiguration configuration,
            IValidator<GoogleLoginDTO> googleLoginValidator,
            ICarOwnerProfileService carOwnerProfileService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _authService = authService;
            _configuration = configuration;
            _googleLoginValidator = googleLoginValidator;
            _carOwnerProfileService = carOwnerProfileService;
        }
        #endregion

        //--------------------- GOOGLE Login ---------------------
        public async Task<ServiceResult<UserDTO>> GoogleLoginAsync(GoogleLoginDTO model)
        {
            if (string.IsNullOrEmpty(model.IdToken))
                return ServiceResult<UserDTO>.Fail("Missing Google ID token.");

            var validation = await _googleLoginValidator.ValidateAsync(model);
            if (!validation.IsValid)
                return ServiceResult<UserDTO>.Fail(string.Join(" , ", validation.Errors.Select(e => e.ErrorMessage)));

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken);
            }
            catch
            {
                return ServiceResult<UserDTO>.Fail("Invalid Google token.");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return ServiceResult<UserDTO>.Fail(string.Join(" , ", createResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, model.Role);

                // 6. Create CarOwnerProfile if role is CarOwner
                if (model.Role.Equals("CAROWNER", StringComparison.OrdinalIgnoreCase))
                {
                    var profile = new CarOwnerProfile
                    {
                        ApplicationUserId = user.Id,
                        ProfileImageUrl = null,
                        PreferredLanguage = PreferredLanguage.English
                    };

                    var createProfileResult = await _carOwnerProfileService.CreateAsync(profile);

                    if (!createProfileResult.Success)
                    {
                        // Rollback user creation if profile creation fails
                        await _userManager.DeleteAsync(user);
                        return ServiceResult<UserDTO>.Fail("Failed to create Car Owner Profile: " + createProfileResult.Message);
                    }
                }

            }

            var jwtToken = await _authService.GenerateJwtTokenAsync(user);
            var refreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            var response = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user),
                Token = jwtToken,
                RefreshToken = refreshToken,
                TokenExpiryTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["jwt:accessTokenExpirationMinutes"])),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };

            return ServiceResult<UserDTO>.Ok(response, "Google login successful.");
        }



    }
}
