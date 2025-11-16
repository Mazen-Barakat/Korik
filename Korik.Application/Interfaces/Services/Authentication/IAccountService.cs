using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Korik.Application
{
    public interface IAccountService
    {
        Task<ServiceResult<UserDTO>> RegisterAsync(RegisterDTO registerDTO, string origin);
        Task SendEmailConfirmationAsync(ApplicationUser user, string origin);
        Task<ServiceResult<string>> ConfirmEmailAsync(string userId, string token);
        Task<ServiceResult<string>> ResendConfirmationEmailAsync(ResendConfirmationEmailDTO model, string origin);
       

        Task<ServiceResult<UserDTO>> LoginAsync(LoginDTO loginDTO);
        
        Task<ServiceResult<UserDTO>> RefreshTokenAsync(RefreshTokenDTO model);
        
        Task<ServiceResult<string>> SendEmailAsync(EmailDTO request);
        

        Task<ServiceResult<string>> ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO, string origin);
        Task<ServiceResult<string>> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);


        Task<ServiceResult<string>> SetPasswordAsync(SetPasswordDTO model, ClaimsPrincipal userPrincipal);



    }
}
