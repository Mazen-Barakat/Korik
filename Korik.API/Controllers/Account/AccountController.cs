using Korik.Application;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Korik.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public AccountController
            (
            IMediator mediator,
            IConfiguration configuration
            )
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        // --------------------- REGISTER ---------------------
        /// <summary>
        /// Registers a new user and sends a confirmation email.
        /// </summary>
        /// <remarks>
        /// <para>🧩 <b>Workflow:</b></para>
        /// <list type="number">
        ///   <item>Validates the user-provided registration data.</item>
        ///   <item>Creates a new user record with the default role <b>'USER'</b>.</item>
        ///   <item>Generates a JWT and refresh token for the user.</item>
        ///   <item>Sends an email confirmation link using FluentEmail.</item>
        /// </list>
        ///
        /// <para>📝 <b>Example Request Body:</b></para>
        /// <code language="json">
        /// {
        ///   "userName": "ahmed123",
        ///   "email": "ahmed@example.com",
        ///   "password": "StrongPass123!",
        ///   "confirmPassword": "StrongPass123!"
        /// }
        /// </code>
        ///
        /// <para>📤 <b>Example Success Response (201 Created):</b></para>
        /// <code language="json">
        /// {
        ///   "success": true,
        ///   "message": "Registration successful. Please confirm your email.",
        ///   "statusCode": 201,
        ///   "data": {
        ///     "id": "c9237a8b-93cb-4f4b-9a44-ff2d33c1c9e3",
        ///     "userName": "ahmed123",
        ///     "email": "ahmed@example.com",
        ///     "roles": [ "USER" ],
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "tokenExpiryTime": "2025-10-04T18:35:12Z",
        ///     "refreshToken": "def50200917a92b5b87f3a...",
        ///     "refreshTokenExpiryTime": "2025-10-11T18:35:12Z"
        ///   }
        /// }
        /// </code>
        ///
        /// <para>❌ <b>Example Failure Response (400 Bad Request):</b></para>
        /// <code language="json">
        /// {
        ///   "success": false,
        ///   "message": "Password must contain at least one uppercase letter | Email is required",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// </code>
        /// </remarks>
        [SwaggerOperation(Summary = "Register new User", Description = "Register new User with Role Called USER.")]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {

            var origin = $"{Request.Scheme}://{Request.Host}";

            var result = await _mediator.Send(new RegisterUserRequest(registerDTO, origin));

            return ApiResponse.FromResult(this , result);
        }



        // --------------------- Confirm EMAIL ---------------------
        /// <summary>
        /// Confirms a user's email address using a verification token.
        /// </summary>
        /// <remarks>
        /// <b>Request Query Example:</b>
        /// <pre>
        /// GET /api/account/ConfirmEmail?userId=12345&token=U29tZUJhc2U2NERlY29kZWRUb2tlbg==
        /// </pre>
        ///
        /// <b>Decoded Token Example:</b>
        /// <pre>
        /// SomeBase64DecodedToken
        /// </pre>
        ///
        /// <b>Response (Success - 302 Redirect):</b>
        /// <pre>
        /// Redirects to frontend confirmation page configured in <b>appsettings.json</b>:
        /// {
        ///   "frontend:confirmEmailURL": "https://mycar.com/account/confirm-success"
        /// }
        /// </pre>
        ///
        /// <b>Response (Failure - 400 or Redirect):</b>
        /// <pre>
        /// {
        ///   "success": false,
        ///   "message": "UserId and Token are required"
        /// }
        ///
        /// OR (Redirects to failed confirmation page)
        /// {
        ///   "frontend:confirmEmailURL": "https://mycar.com/account/confirm-failed"
        /// }
        /// </pre>
        /// </remarks>
        /// <param name="userId">The ID of the user whose email is to be confirmed.</param>
        /// <param name="token">The base64-encoded email confirmation token.</param>
        /// <returns>
        /// Returns a redirect response to the frontend (success or failure page),
        /// or a JSON error if parameters are invalid.
        /// </returns>
        /// <response code="302">Redirects to frontend success or failure page.</response>
        /// <response code="400">Missing or invalid userId/token.</response>
        /// <response code="404">User not found.</response>
        [SwaggerOperation(
            Summary = "Confirm user email",
            Description = "Verifies the user's email using a userId and confirmation token."
        )]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _mediator.Send(new ConfirmEmailRequest(userId, token));

            // Redirect to frontend (based on success/failure)
            var redirectUrl = result.Success
                ? _configuration["frontend:confirmEmailURL"]
                : _configuration["frontend:confirmEmailFailedURL"];

            // If using frontend only, redirect:
            return Redirect(redirectUrl);
        }



        // --------------------- RESEND CONFIRMATION EMAIL ---------------------
        /// <summary>
        /// Resends the email confirmation link to a user who has not yet verified their email.
        /// </summary>
        /// <remarks>
        /// <b>Request Body Example (JSON):</b>
        /// <pre>
        /// {
        ///   "email": "user@example.com"
        /// }
        /// </pre>
        ///
        /// <b>Response (200 OK - Success):</b>
        /// <pre>
        /// {
        ///   "success": true,
        ///   "message": "✅ Confirmation email resent successfully.",
        ///   "data": null,
        ///   "statusCode": 200
        /// }
        /// </pre>
        ///
        /// <b>Response (400 Bad Request - Validation Errors):</b>
        /// <pre>
        /// {
        ///   "success": false,
        ///   "message": "Email field is required | Invalid email format",
        ///   "data": null,
        ///   "statusCode": 400
        /// }
        /// </pre>
        ///
        /// </remarks>
        /// <param name="model">The email address of the user requesting a new confirmation link.</param>
        /// <returns>
        /// Returns a <see cref="ServiceResult{string}"/> response indicating success or failure.
        /// </returns>
        /// <response code="200">Email resent successfully.</response>
        /// <response code="400">Validation failed or invalid input.</response>
        [HttpPost("ResendConfirmationEmail")]
        [SwaggerOperation(
            Summary = "Resend confirmation email",
            Description = "Resends the email confirmation link to the user if their email is not yet confirmed."
        )]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDTO model)
        {
            var origin = $"{Request.Scheme}://{Request.Host}";

            var result = await _mediator.Send(new ResendConfirmEmailRequest(model , origin));

            return ApiResponse.FromResult(this, result);

        }



        // --------------------- LOGIN ---------------------
        /// <summary>
        /// Authenticates a user and returns JWT and refresh tokens.
        /// </summary>
        /// <remarks>
        /// <para>🧩 <b>Workflow:</b></para>
        /// <list type="number">
        ///   <item>Validates the provided login credentials.</item>
        ///   <item>Checks if the user exists and verifies the password.</item>
        ///   <item>Ensures the user's email has been confirmed.</item>
        ///   <item>Generates a new JWT and refresh token for authenticated sessions.</item>
        ///   <item>Updates the user's refresh token details in the database.</item>
        /// </list>
        ///
        /// <para>📝 <b>Example Request Body:</b></para>
        /// <code language="json">
        /// {
        ///   "email": "ahmed@example.com",
        ///   "password": "StrongPass123!"
        /// }
        /// </code>
        ///
        /// <para>📤 <b>Example Success Response (200 OK):</b></para>
        /// <code language="json">
        /// {
        ///   "success": true,
        ///   "message": "Login successful",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "id": "c9237a8b-93cb-4f4b-9a44-ff2d33c1c9e3",
        ///     "userName": "ahmed123",
        ///     "email": "ahmed@example.com",
        ///     "roles": [ "USER" ],
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "tokenExpiryTime": "2025-10-04T19:00:12Z",
        ///     "refreshToken": "def50200917a92b5b87f3a...",
        ///     "refreshTokenExpiryTime": "2025-10-11T19:00:12Z"
        ///   }
        /// }
        /// </code>
        ///
        /// <para>❌ <b>Example Failure Response (400 Bad Request):</b></para>
        /// <code language="json">
        /// {
        ///   "success": false,
        ///   "message": "Invalid Email or Password",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// </code>
        ///
        /// <para>❌ <b>Example Failure Response (401 Unauthorized):</b></para>
        /// <code language="json">
        /// {
        ///   "success": false,
        ///   "message": "Email not confirmed. Please check your email.",
        ///   "statusCode": 401,
        ///   "data": null
        /// }
        /// </code>
        /// </remarks>
        [SwaggerOperation(Summary = "Login User", Description = "Login User and give it a new JWT Token configuration.")]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _mediator.Send(new Application.LoginRequest(loginDTO) );
            
            return ApiResponse.FromResult(this, result);
        }



        // --------------------- REFRESH TOKEN ---------------------
        /// <summary>
        /// Refreshes the user's JWT token using a valid refresh token.
        /// </summary>
        /// <remarks>
        /// <para>🧩 <b>Workflow:</b></para>
        /// <list type="number">
        ///   <item>Receives the user's <c>userId</c> and <c>refreshToken</c> in the request body.</item>
        ///   <item>Verifies that the user exists in the system.</item>
        ///   <item>Checks that the refresh token is valid and not expired.</item>
        ///   <item>Generates a new JWT access token.</item>
        ///   <item>Optionally rotates the refresh token (issues a new one for enhanced security).</item>
        ///   <item>Returns the updated tokens and expiry times.</item>
        /// </list>
        ///
        /// <para>📝 <b>Example Request Body:</b></para>
        /// <code language="json">
        /// {
        ///   "userId": "c9237a8b-93cb-4f4b-9a44-ff2d33c1c9e3",
        ///   "refreshToken": "def50200917a92b5b87f3a..."
        /// }
        /// </code>
        ///
        /// <para>📤 <b>Example Success Response (200 OK):</b></para>
        /// <code language="json">
        /// {
        ///   "success": true,
        ///   "message": "Token refreshed successfully",
        ///   "statusCode": 200,
        ///   "data": {
        ///     "id": "c9237a8b-93cb-4f4b-9a44-ff2d33c1c9e3",
        ///     "userName": "ahmed123",
        ///     "email": "ahmed@example.com",
        ///     "roles": [ "USER" ],
        ///     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///     "tokenExpiryTime": "2025-10-04T20:12:45Z",
        ///     "refreshToken": "new_refresh_token_abc123...",
        ///     "refreshTokenExpiryTime": "2025-10-11T20:12:45Z"
        ///   }
        /// }
        /// </code>
        ///
        /// <para>❌ <b>Example Failure Response (400 Bad Request):</b></para>
        /// <code language="json">
        /// {
        ///   "success": false,
        ///   "message": "Invalid request",
        ///   "statusCode": 400,
        ///   "data": null
        /// }
        /// </code>
        ///
        /// </remarks>

        [SwaggerOperation(Summary = "Refresh JWT Token", Description = "Refresh JWT using a valid refresh token.")]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO model)
        {
            var result = await _mediator.Send(new RefreshTokenRequest(model));

            return ApiResponse.FromResult(this, result);
        }



        // --------------------- Send EMAIL ---------------------
        /// <summary>
        /// Sends an email to the specified recipient.
        /// </summary>
        /// <remarks>
        /// <b>Request JSON Example:</b>
        /// <pre>
        /// {
        ///   "to": "user@example.com",
        ///   "subject": "Welcome to MyCar",
        ///   "body": "<h1>Hello!</h1><p>Thank you for joining MyCar.</p>"
        /// }
        /// </pre>
        ///
        /// <b>Response JSON Example (Success - 200):</b>
        /// <pre>
        /// {
        ///   "success": true,
        ///   "message": "Email sent successfully.",
        ///   "data": "Email sent successfully.",
        ///   "statusCode": 200
        /// }
        /// </pre>
        ///
        /// <b>Response JSON Example (Validation Failed - 400):</b>
        /// <pre>
        /// {
        ///   "success": false,
        ///   "message": "To field is required | Subject cannot be empty",
        ///   "data": null,
        ///   "statusCode": 400
        /// }
        /// </pre>
        ///
        /// <b>Response JSON Example (Internal Error - 500):</b>
        /// <pre>
        /// {
        ///   "success": false,
        ///   "message": "Error sending email | SMTP connection failed",
        ///   "data": null,
        ///   "statusCode": 500
        /// }
        /// </pre>
        /// </remarks>
        /// <param name="request">The email request containing recipient address, subject, and body.</param>
        /// <returns>
        /// Returns a <see cref="ServiceResult{T}"/> object containing the operation result.
        /// </returns>
        /// <response code="200">Email sent successfully.</response>
        /// <response code="400">Validation errors in the email request.</response>
        /// <response code="500">An internal error occurred while sending the email.</response>
        [SwaggerOperation(
            Summary = "Send an email",
            Description = "Accepts an EmailRequestDTO and sends an email to the specified recipient."
        )]
        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDTO model)
        {
            var result = await _mediator.Send(new SendEmailRequest(model));

            return ApiResponse.FromResult(this, result);
        }


        // --------------------- FORGOT PASSWORD ---------------------
        /// <summary>
        /// Sends a password reset email to the specified user.
        /// </summary>
        /// <remarks>
        /// <b>Request Body Example (JSON):</b>
        /// <pre>
        /// {
        ///   "email": "user@example.com"
        /// }
        /// </pre>
        ///
        /// <b>Response (200 OK - Success):</b>
        /// <pre>
        /// {
        ///   "success": true,
        ///   "message": "Password reset link has been sent to your email.",
        ///   "data": null,
        ///   "statusCode": 200
        /// }
        /// </pre>
        ///
        /// <b>Response (400 Bad Request - Validation Error):</b>
        /// <pre>
        /// {
        ///   "success": false,
        ///   "message": "Email field is required | Invalid email format",
        ///   "data": null,
        ///   "statusCode": 400
        /// }
        /// </pre>
        ///
        /// </remarks>
        /// <param name="forgotPasswordDTO">The user's email address requesting a password reset.</param>
        /// <returns>
        /// A <see cref="ServiceResult{string}"/> containing a success or failure message.
        /// </returns>
        /// <response code="200">Password reset email sent successfully.</response>
        /// <response code="400">Validation failed (invalid or missing email).</response>
        [SwaggerOperation(
            Summary = "Request a password reset",
            Description = "Generates a password reset link for the user and sends it via email."
        )]
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var origin = $"{_configuration["frontend:confirmPasswordURL"]}";

            var result = await _mediator.Send(new ForgotPasswordRquest(model , origin));

            return ApiResponse.FromResult(this, result);
        }



        // ---------------------Get RESET PASSWORD ---------------------
        /// <summary>
        /// Redirects the user to the frontend password reset page with the provided token and email.
        /// </summary>
        /// <remarks>
        /// This endpoint is called when a user clicks the **"Reset Password"** link from their email.
        ///
        /// The backend extracts the `token` and `email` from the query parameters and redirects the user to the
        /// configured frontend URL where the actual password reset form resides.
        ///
        /// <b>Request Example (GET):</b>
        /// <pre>
        /// GET /api/account/ResetPassword?token=QWxhZGRpbjpvcGVuIHNlc2FtZQ==&email=user@example.com
        /// </pre>
        ///
        /// <b>Successful Redirect Example:</b>
        /// <pre>
        /// Redirects to: https://frontend-domain.com/reset-password?token=QWxhZGRpbjpvcGVuIHNlc2FtZQ==&email=user@example.com
        /// </pre>
        ///
        /// <b>Response (302 Redirect - Success):</b>
        /// Redirects to the frontend password reset page with query parameters.
        ///
        /// <b>Response (302 Redirect - Missing Query Parameters):</b>
        /// Redirects to the frontend failure confirmation page if the token or email is missing.
        /// </remarks>
        /// <param name="token">The Base64-encoded password reset token sent via email.</param>
        /// <param name="email">The user's email address.</param>
        /// <returns>Redirects to the frontend password reset page or failure page.</returns>
        /// <response code="302">Redirects the user to the frontend reset password or failure page.</response>
        [SwaggerOperation(
            Summary = "Redirect to Frontend Reset Password Page",
            Description = @"
                        This endpoint is triggered when a user clicks the reset password link from their email.  
                        It validates the query parameters (`token`, `email`) and redirects the user to the frontend reset page."
         )]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword([FromQuery] string token, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
                return Redirect(_configuration["frontend:confirmPasswordURL"]); // Redirect to frontend Failed To Confirm confirmation page

            else
            {
                var frontendFailedURL = $"{_configuration["frontend:confirmPasswordURL"]}/?{token = token}&{email = email}";
                return Redirect(frontendFailedURL); // Redirect to frontend confirmation page
            }
        }



        // ---------------------Post RESET PASSWORD ---------------------
        /// <summary>
        /// Resets the user's password using a valid reset token.
        /// </summary>
        /// <remarks>
        /// This endpoint is called by the frontend after the user submits their **new password** form.
        ///
        /// The user must provide:
        /// - Their registered email address
        /// - The Base64-encoded reset token (sent to them via email)
        /// - Their new password
        ///
        /// <b>Request Example (JSON):</b>
        /// <pre>
        /// {
        ///   "email": "user@example.com",
        ///   "passwordResetToken": "U29tZUJhc2U2NEVuY29kZWRUb2tlbg==",
        ///   "newPassword": "NewStrongPassword123!"
        /// }
        /// </pre>
        ///
        /// <b>Successful Response (200):</b>
        /// <pre>
        /// {
        ///   "success": true,
        ///   "message": "✅ Password has been reset successfully."
        /// }
        /// </pre>
        ///
        /// <b>Failed Response (400):</b>
        /// <pre>
        /// {
        ///   "success": false,
        ///   "message": "Invalid password reset token format."
        /// }
        /// </pre>
        ///
        /// <b>Failure Reasons:</b>
        /// - Invalid or expired token  
        /// - User not found  
        /// - Weak password that doesn't meet identity requirements  
        /// - Invalid request body (fails validation)
        /// </remarks>
        /// <param name="resetPasswordDTO">The DTO containing email, Base64 token, and new password.</param>
        /// <returns>Returns a success message or validation errors.</returns>
        /// <response code="200">Password has been reset successfully.</response>
        /// <response code="400">Invalid token, missing fields, or failed password reset.</response>
        [SwaggerOperation(
            Summary = "Reset user password",
            Description = "Resets the password for the user using the token sent in the email. Requires the Base64-encoded token and new password."
        )]

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {

            var result = await _mediator.Send(new Application.ResetPasswordRequest(model));


            return ApiResponse.FromResult(this, result);
        }


        // --------------------- GOOGLE LOGIN ---------------------
        [HttpPost("Google-login")]
        [SwaggerOperation(
            Summary = "Register user using Google account",
            Description = "Accepts a Google ID token, validates it, and returns a JWT + Refresh token."
        )]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleLoginDTO model)
        {
            var result = await _mediator.Send(new GoogleLoginRequest(model));

            return ApiResponse.FromResult(this, result);
        }



        // --------------------- SET PASSWORD ---------------------
        [Authorize]
        [HttpPost("set-password")]
        [SwaggerOperation(
            Summary = "Set password for users registered via external providers",
            Description = "Allows users who registered using Google/Facebook to set a local password for their account."
        )]
        public async Task<IActionResult> SetPassword([FromBody] SetPasswordDTO model)
        {
            var result = await _mediator.Send(new SetPasswordRequest(model , User));
            return ApiResponse.FromResult(this, result);
        }

    }
}
