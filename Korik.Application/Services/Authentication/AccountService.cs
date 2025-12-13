using AutoMapper;
using FluentEmail.Core;
using FluentValidation;
using Google.Apis.Auth;
using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public class AccountService : IAccountService
    {
        #region Dependency Injection
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IFluentEmail _fluentEmail;
        private readonly IValidator<RegisterDTO> _registerValidator;
        private readonly IValidator<ResendConfirmationEmailDTO> _resendEmailValidator;
        private readonly IValidator<LoginDTO> _loginValidator;
        private readonly IValidator<EmailDTO> _emailValidator;
        private readonly IValidator<ForgotPasswordDTO> _forgotPasswordValidator;
        private readonly IValidator<ResetPasswordDTO> _resetPasswordValidator;
        private readonly IValidator<GoogleLoginDTO> _googleLoginValidator;
        private readonly IValidator<SetPasswordDTO> _setPasswordValidator;
        private readonly ICarOwnerProfileService _carOwnerProfileService;
        private readonly IWorkShopProfileService _workShopProfileService;


        public AccountService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IAuthService authService,
            IConfiguration configuration,
            IFluentEmail fluentEmail,
            IValidator<RegisterDTO> registerValidator,
            IValidator<ResendConfirmationEmailDTO> resendEmailValidator,
            IValidator<LoginDTO> loginValidator,
            IValidator<EmailDTO> emailValidator,
            IValidator<ForgotPasswordDTO> forgotPasswordValidator,
            IValidator<ResetPasswordDTO> resetPasswordValidator,
            IValidator<GoogleLoginDTO> googleLoginValidator,
            IValidator<SetPasswordDTO> setPasswordValidator,
            ICarOwnerProfileService carOwnerProfileService,
            IWorkShopProfileService workShopProfileService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _authService = authService;
            _configuration = configuration;
            _fluentEmail = fluentEmail;
            _registerValidator = registerValidator;
            _resendEmailValidator = resendEmailValidator;
            _loginValidator = loginValidator;
            _emailValidator = emailValidator;
            _forgotPasswordValidator = forgotPasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _googleLoginValidator = googleLoginValidator;
            _setPasswordValidator = setPasswordValidator;
            _carOwnerProfileService = carOwnerProfileService;
            _workShopProfileService = workShopProfileService;
        }
        #endregion



        // --------------------- REGISTER ---------------------
        public async Task<ServiceResult<UserDTO>> RegisterAsync(RegisterDTO registerDTO, string origin)
        {
            if (registerDTO == null)
                return ServiceResult<UserDTO>.Fail("model body cannot be null");

            // 1. Validate input
            var validation = await _registerValidator.ValidateAsync(registerDTO);
            if (!validation.IsValid)
            {
                var errors = string.Join(",", validation.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<UserDTO>.Fail(errors);
            }

            // 2. Map DTO → Entity
            var user = _mapper.Map<ApplicationUser>(registerDTO);

            // 4. Create User
            var createResult = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(",", createResult.Errors.Select(e => e.Description));
                return ServiceResult<UserDTO>.Fail(errors);
            }

            // 5. Assign Role
            await _userManager.AddToRoleAsync(user, registerDTO.Role);

            // 6. Create CarOwnerProfile if role is CarOwner
            if (registerDTO.Role.Equals("CAROWNER", StringComparison.OrdinalIgnoreCase))
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


            // 6. Create WorkShopProfile if role is WORKSHOP
            if (registerDTO.Role.Equals("WORKSHOP", StringComparison.OrdinalIgnoreCase))
            {
                var profile = new WorkShopProfile
                {
                    ApplicationUserId = user.Id,
                    Name = "New Workshop",
                    Description = string.Empty,
                    WorkShopType = WorkShopType.Independent,
                    Country = string.Empty,
                    Governorate = string.Empty,
                    City = string.Empty,
                    PhoneNumber = string.Empty,
                    Latitude = 0,
                    Longitude = 0,
                    NumbersOfTechnicians = 0,
                    LicenceImageUrl = string.Empty,   // REQUIRED FIELD
                    LogoImageUrl = null               // optional
                };


                var createProfileResult = await _workShopProfileService.CreateAsync(profile);

                if (!createProfileResult.Success)
                {
                    // Rollback user creation if profile creation fails
                    await _userManager.DeleteAsync(user);
                    return ServiceResult<UserDTO>.Fail("Failed to create WorkShop Profile: " + createProfileResult.Message);
                }
            }



            // 6. Generate Tokens
            var token = await _authService.GenerateJwtTokenAsync(user);
            var refreshToken = _authService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Example: 7 days validity
            await _userManager.UpdateAsync(user);

            // 7. Send Email Confirmation
            await SendEmailConfirmationAsync(user, origin);

            // 8. Build Response DTO
            var UserDTO = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user),
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiryTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };

            return ServiceResult<UserDTO>.Created(UserDTO, "Registration successful. Please confirm your email.");
        }

        public async Task SendEmailConfirmationAsync(ApplicationUser user, string origin)
        {
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);


            // must encode for URL

            // Encode
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(emailToken));


            var confirmationLink = $"{origin}/api/account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            await _fluentEmail
                .To(user.Email)
                .Subject("Confirm your email for My Car App")
              .Body($@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: Inter, -apple-system, BlinkMacSystemFont, Segoe UI, Roboto, sans-serif; background: linear-gradient(135deg, #f8fafc 0%, #e5e7eb 100%);'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td align='center' style='padding: 40px 20px;'>
                <!-- Main Container -->
                <table role='presentation' style='max-width: 600px; width: 100%; background: #ffffff; border-radius: 16px; box-shadow: 0 20px 60px rgba(0,0,0,0.12); overflow: hidden;'>
                    
                    <!-- Header with Brand Colors -->
                    <tr>
                        <td style='background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%); padding: 40px 30px; text-align: center;'>
                            <div style='background: rgba(255,255,255,0.15); border-radius: 50%; width: 80px; height: 80px; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; backdrop-filter: blur(10px);'>
                                <span style='font-size: 40px;'>🚗</span>
                            </div>
                            <h1 style='margin: 0; color: #ffffff; font-size: 28px; font-weight: 700; letter-spacing: -0.5px;'>KORIEK</h1>
                            <p style='margin: 8px 0 0; color: rgba(255,255,255,0.9); font-size: 14px; font-weight: 500;'>Your Trusted Auto Service Platform</p>
                        </td>
                    </tr>
                    
                    <!-- Welcome Content -->
                    <tr>
                        <td style='padding: 40px 35px;'>
                            <h2 style='margin: 0 0 16px; color: #1e3a5f; font-size: 24px; font-weight: 700; text-align: center;'>
                                Welcome Aboard, {user.UserName}! 👋
                            </h2>
                            <p style='margin: 0 0 24px; color: #4b5563; font-size: 16px; line-height: 1.6; text-align: center;'>
                                We're excited to have you join the KORIEK community! You're just one step away from accessing premium auto services, trusted workshops, and seamless vehicle maintenance.
                            </p>
                            
                            <!-- Confirmation Button -->
                            <table role='presentation' style='width: 100%; margin: 32px 0;'>
                                <tr>
                                    <td align='center'>
                                        <a href='{confirmationLink}' style='display: inline-block; padding: 16px 40px; background: linear-gradient(135deg, #ef4444 0%, #dc2626 100%); color: #ffffff; text-decoration: none; border-radius: 12px; font-size: 16px; font-weight: 700; box-shadow: 0 8px 24px rgba(239,68,68,0.25); transition: all 0.3s ease;'>
                                            ✅ Confirm My Email Address
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <!-- Alternative Link -->
                            <p style='margin: 24px 0 0; padding: 20px; background: #f8fafc; border-radius: 8px; color: #6b7280; font-size: 13px; line-height: 1.5; text-align: center; border-left: 4px solid #ef4444;'>
                                <strong style='color: #1e3a5f;'>Button not working?</strong><br>
                                Copy and paste this link into your browser:<br>
                                <a href='{confirmationLink}' style='color: #ef4444; word-break: break-all;'>{confirmationLink}</a>
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Features Section -->
                    <tr>
                        <td style='padding: 0 35px 40px;'>
                            <div style='background: linear-gradient(135deg, #fef3f2 0%, #fee2e2 100%); border-radius: 12px; padding: 24px; border: 1px solid #fecaca;'>
                                <h3 style='margin: 0 0 16px; color: #1e3a5f; font-size: 18px; font-weight: 700; text-align: center;'>What's Waiting For You</h3>
                                <table role='presentation' style='width: 100%;'>
                                    <tr>
                                        <td style='padding: 8px 0;'>
                                            <span style='color: #ef4444; font-size: 18px; margin-right: 12px;'>🔧</span>
                                            <span style='color: #4b5563; font-size: 14px;'>Book services at certified workshops</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0;'>
                                            <span style='color: #ef4444; font-size: 18px; margin-right: 12px;'>📅</span>
                                            <span style='color: #4b5563; font-size: 14px;'>Manage appointments & vehicle records</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0;'>
                                            <span style='color: #ef4444; font-size: 18px; margin-right: 12px;'>💰</span>
                                            <span style='color: #4b5563; font-size: 14px;'>Secure payments & digital wallet</span>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style='padding: 8px 0;'>
                                            <span style='color: #ef4444; font-size: 18px; margin-right: 12px;'>⭐</span>
                                            <span style='color: #4b5563; font-size: 14px;'>Read reviews & rate your experiences</span>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='background: #1e3a5f; padding: 30px 35px; text-align: center;'>
                            <p style='margin: 0 0 16px; color: rgba(255,255,255,0.9); font-size: 14px;'>
                                Need help? We're here for you!
                            </p>
                            <p style='margin: 0 0 20px; color: rgba(255,255,255,0.7); font-size: 13px;'>
                                📧 support@koriek.com | 📞 +20 123 456 7890
                            </p>
                            <div style='border-top: 1px solid rgba(255,255,255,0.1); padding-top: 20px; margin-top: 20px;'>
                                <p style='margin: 0 0 8px; color: rgba(255,255,255,0.5); font-size: 12px;'>
                                    © 2025 KORIEK. All rights reserved.
                                </p>
                                <p style='margin: 0; color: rgba(255,255,255,0.5); font-size: 11px;'>
                                    This email was sent to verify your account registration.
                                </p>
                            </div>
                        </td>
                    </tr>
                    
                </table>
                
                <!-- Security Notice -->
                <table role='presentation' style='max-width: 600px; width: 100%; margin-top: 20px;'>
                    <tr>
                        <td style='text-align: center; padding: 0 20px;'>
                            <p style='margin: 0; color: #6b7280; font-size: 12px; line-height: 1.5;'>
                                🔒 This link will expire in 24 hours for your security.<br>
                                If you didn't create a KORIEK account, please ignore this email.
                            </p>
                        </td>
                    </tr>
                </table>
                
            </td>
        </tr>
    </table>
</body>
</html>", true)
                .SendAsync();
        }

        public async Task<ServiceResult<string>> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return ServiceResult<string>.Fail("UserId and Token are required");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ServiceResult<string>.Fail("User not found");

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            }
            catch
            {
                return ServiceResult<string>.Fail("Invalid token format.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
                return ServiceResult<string>.Ok("Email confirmed successfully.");

            var errors = string.Join(",", result.Errors.Select(e => e.Description));
            return ServiceResult<string>.Fail(errors);
        }

        public async Task<ServiceResult<string>> ResendConfirmationEmailAsync(ResendConfirmationEmailDTO model, string origin)
        {
            #region Validation
            var validationResult = await _resendEmailValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(" ,", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<string>.Fail(errors);
            }
            #endregion

            #region User Checks
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return ServiceResult<string>.Fail("User not found.");

            if (user.EmailConfirmed)
                return ServiceResult<string>.Fail("Email is already confirmed.");
            #endregion

            #region Send Email

            await SendEmailConfirmationAsync(user, origin);
            return ServiceResult<string>.Ok("Confirmation email resent successfully.");
            #endregion
        }



        // --------------------- LOGIN ---------------------
        public async Task<ServiceResult<UserDTO>> LoginAsync(LoginDTO loginDTO)
        {
            // Null check
            if (loginDTO == null)
                return ServiceResult<UserDTO>.Fail("Request body cannot be null");

            // Validate DTO
            var validation = await _loginValidator.ValidateAsync(loginDTO);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<UserDTO>.Fail(errors);
            }


            // Find user
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return ServiceResult<UserDTO>.Fail("Invalid Email or Password");

            // Validate password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!isPasswordValid)
                return ServiceResult<UserDTO>.Fail("Invalid Email or Password");

            // Check email confirmation
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return ServiceResult<UserDTO>.Fail("Email not confirmed. Please check your email.");

            // Generate JWT and Refresh Token
            var token = await _authService.GenerateJwtTokenAsync(user);
            var refreshToken = _authService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            // Build DTO response
            var loginResponse = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user),
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiryTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };

            return ServiceResult<UserDTO>.Ok(loginResponse, "Login successful");
        }



        // --------------------- REFRESH TOKEN ---------------------
        public async Task<ServiceResult<UserDTO>> RefreshTokenAsync(RefreshTokenDTO model)
        {
            // 1️ Validate Request

            if (model == null || string.IsNullOrEmpty(model.UserId) || string.IsNullOrEmpty(model.RefreshToken))
                return ServiceResult<UserDTO>.Fail("Invalid request.");

            // 2️ Find User
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return ServiceResult<UserDTO>.Fail("User not found.");

            // 3️ Validate Refresh Token
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return ServiceResult<UserDTO>.Fail("Invalid or expired refresh token.");

            // 4️ Generate new JWT token
            var newJwt = await _authService.GenerateJwtTokenAsync(user);

            // 5️ Rotate Refresh Token (for better security)
            var newRefreshToken = _authService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // example: valid for 7 days
            await _userManager.UpdateAsync(user);

            // 6️ Prepare Response DTO
            var response = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user),
                Token = newJwt,
                RefreshToken = newRefreshToken,
                TokenExpiryTime = DateTime.Now.AddMinutes(
                    Convert.ToInt32(_configuration["jwt:accessTokenExpirationMinutes"])
                ),
                RefreshTokenExpiryTime = user.RefreshTokenExpiryTime
            };

            // 7️ Return Success
            return ServiceResult<UserDTO>.Ok(response, "Token refreshed successfully.");
        }


        // --------------------- SEND EMAIL ---------------------
        public async Task<ServiceResult<string>> SendEmailAsync(EmailDTO model)
        {
            // Validate input
            var validationResult = _emailValidator.Validate(model);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<string>.Fail(errors);
            }

            // Send email
            var response = await _fluentEmail
                .To(model.To)
                .Subject(model.Subject)
                .Body(model.Body, isHtml: true)
                .SendAsync();

            // Return result
            if (response.Successful)
                return ServiceResult<string>.Ok("Email sent successfully.");

            var serverErrors = string.Join(", ", response.ErrorMessages);
            return ServiceResult<string>.Fail(serverErrors);
        }

    
        // --------------------- FORGOT PASSWORD ---------------------
        public async Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO, string origin)
        {
            #region Validation
            var validationResult = await _forgotPasswordValidator.ValidateAsync(forgotPasswordDTO);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<string>.Fail(errors);
            }
            #endregion

            #region Check User
            var user = await _userManager.FindByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
                return ServiceResult<string>.Fail("User not found.");
            #endregion

            #region Generate Token & Link
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

            // Example: frontend URL (configured in appsettings)
            var resetLink = $"{origin}/reset-password?email={user.Email}&token={encodedToken}";
            #endregion

            #region Send Email
            var subject = "Password Reset Request - My Car App";
            var body = $@"
                            <div style='font-family: Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 20px;'>
                              <div style='max-width: 500px; margin: auto; background: #fff; padding: 25px; border-radius: 8px; box-shadow: 0 2px 6px rgba(0,0,0,0.1);'>
                                <h2 style='color: #1E88E5;'>Reset Your Password</h2>
                                <p>Hi <strong>{user.UserName}</strong>,</p>
                                <p>You requested to reset your password for <strong>My Car App</strong>. Click the button below to continue:</p>
    
                                <p style='text-align: center; margin: 25px 0;'>
                                  <a href='{resetLink}' style='background-color: #1E88E5; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 6px; font-weight: bold;'>
                                    Reset Password
                                  </a>
                                </p>

                                <p>If you didn’t request this, you can safely ignore this email.</p>
                                <p style='color: #666;'>— The MyCar App Team 🚗</p>
                              </div>
                            </div>";
            var response = await _fluentEmail
                .To(user.Email)
                .Subject(subject)
                .Body(body, isHtml: true)
                .SendAsync();

            if (!response.Successful)
                return ServiceResult<string>.Fail("Failed to send reset email.");
            #endregion

            return ServiceResult<string>.Ok("Password reset link has been sent to your email.");
        }

        // --------------------- RESET PASSWORD ---------------------
        public async Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            #region Validation
            var validationResult = await _resetPasswordValidator.ValidateAsync(resetPasswordDTO);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<string>.Fail(errors);
            }
            #endregion

            #region Check User
            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
                return ServiceResult<string>.Fail("User not found");
            #endregion

            #region Decode Token
            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(resetPasswordDTO.PasswordResetToken));
            }
            catch
            {
                return ServiceResult<string>.Fail("Invalid password reset token format.");
            }
            #endregion

            #region Reset Password
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDTO.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return ServiceResult<string>.Fail(errors);
            }
            #endregion

            #region Success
            return ServiceResult<string>.Ok("✅ Password has been reset successfully.");
            #endregion
        }



        // --------------------- SET PASSWORD ---------------------
        public async Task<ServiceResult<string>> SetPasswordAsync(SetPasswordDTO model, ClaimsPrincipal userPrincipal)
        {
            // Validate input
            var validation = await _setPasswordValidator.ValidateAsync(model);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return ServiceResult<string>.Fail(errors);
            }

            // Get current user from token
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null)
                return ServiceResult<string>.Fail("User not found.");

            // Check if user already has a password
            if (await _userManager.HasPasswordAsync(user))
                return ServiceResult<string>.Fail("User already has a password. Use Change Password instead.");

            // Add new password
            var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                return ServiceResult<string>.Fail(errors);
            }

            return ServiceResult<string>.Ok("Password set successfully. You can now log in using email and password.");
        }




    }
}
