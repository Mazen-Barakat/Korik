using Korik.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace Korik.Application
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService
            (
             IConfiguration configuration,
             UserManager<ApplicationUser> userManager
            )
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            #region Claims

            //Add User Claims
            IList<Claim>? userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            //Add User Roles As Claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add predefined Claims
            userClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

            #endregion

            #region Create Signing Credentials (Key + Algo)
            SigningCredentials signingCredentialsObj = new SigningCredentials
                (
                    key: new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:key"])),  // Symmetric Key
                    algorithm: SecurityAlgorithms.HmacSha256 // Algorithm
                );
            #endregion

            //Create JWT Token design  
            //Add Token Payload (Claims) (predefined and user claims) in Ctor - 4th overload 
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                    issuer: _configuration["jwt:issuer"],          // Typically your Backend URL
                    audience: _configuration["jwt:audience"],      // Typically your FrontEnd URL
                    expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["jwt:accessTokenExpirationMinutes"])),    // Expiration Time
                    claims: userClaims,                            // Payload
                    signingCredentials: signingCredentialsObj      // Sign the Token
                );

            //Generate Token As String
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
