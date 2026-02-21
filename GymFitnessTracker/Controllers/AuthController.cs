using Azure.Core;
using GymFitnessTracker.ErrorHandling;
using GymFitnessTracker.Models.Domain;
using GymFitnessTracker.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace GymFitnessTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ITokenRepository _tokenRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;


        public AuthController(UserManager<ApplicationUser> userManager, ILogger<AuthController> logger, IEmailSender emailSender, ITokenRepository tokenRepository, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _tokenRepository = tokenRepository;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)//[FromBody] RegisterUserRequest registerUserRequest)
        {
            try
            {
                // check if user already exists

                var existingUser = await _userManager.FindByEmailAsync(registerUserRequest.Email);
                var existingUsername = await _userManager.FindByNameAsync(registerUserRequest.Username);

                if (existingUser != null)
                {
                    //return BadRequest(error: "Email already exists");
                    return BadRequest(
                        new {message = "Email already exists"}
                        );
                }

                if (existingUsername != null)
                {
                    //return BadRequest(error: "Email already exists");
                    return BadRequest(
                        new { message = "Username already exists" }
                        );
                }

                
                if (string.IsNullOrWhiteSpace(registerUserRequest.Username) || registerUserRequest.Username.Contains(" "))
                {
                    return BadRequest(
                        new {message = "Username cannot contain spaces"}
                        );
                }

                var identityUser = new ApplicationUser { 
                    UserName = registerUserRequest.Username,
                    //UserName = registerUserRequest.Email,
                    Email = registerUserRequest.Email,
                    InAppName = registerUserRequest.InAppName,
                    Gender = registerUserRequest.Gender,
                    VerificationPin = new Random().Next(100000,999999).ToString(),
                    VerificationPinExpiry = DateTime.UtcNow.AddMinutes(1),
                    LoginProvider = "Email" // Set login provider for email registration
                };

                // Handle profile picture
                /*if(registerUserRequest.ProfilePicture != null && registerUserRequest.ProfilePicture.Length > 0) 
                {
                    var rootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var uploadsPath = Path.Combine(rootPath, "uploads", "profile-pictures");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(registerUserRequest.ProfilePicture.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await registerUserRequest.ProfilePicture.CopyToAsync(stream);
                    }

                    identityUser.ProfilePictureUrl = $"/uploads/profile-pictures/{fileName}";
                }*/


                // create user
                var identityResult = await _userManager.CreateAsync(identityUser, registerUserRequest.Password);

                if (identityResult.Succeeded)
                {
                    // add roles to user
                    if(registerUserRequest.Roles != null && registerUserRequest.Roles.Any())
                    {
                        foreach (var role in registerUserRequest.Roles)
                        {
                            if(!await _roleManager.RoleExistsAsync(role))
                            {
                                return BadRequest(new { message = $"Role '{role}' does not exist" });
                            }
                        }
                        identityResult = await _userManager.AddToRolesAsync(identityUser, registerUserRequest.Roles);
                        if (identityResult.Succeeded)
                        {
                            // send verificationMail
                            await _emailSender.SendEmailAsync(
                                registerUserRequest.Email,
                                "Verification code",
                                $"Your verification code is: { identityUser.VerificationPin }"
                                );

                            //return Ok("Please check your email for verification code");
                            return Ok(
                                new { message = "Please check your email for verification code" }
                                );

                        }
                    }
                }

                /*return BadRequest(
                    new {Message = identityResult.Errors}
                    );*/
                return BadRequest(identityResult.Errors.Select(e => e.Description));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost]
        [Route("VerifyPin")]
        public async Task<IActionResult> VeifyPin(VerifyPinRequest verifyPinRequest)
        {
            var timeNow = DateTime.UtcNow;
            var user = await _userManager.FindByEmailAsync(verifyPinRequest.Email);
            if (user == null)
            {
                //return BadRequest(error: "Invalid user");
                return NotFound(
                    new {message = "User not found" }
                    );
            }

            var differenceInMinutes = timeNow.Subtract((DateTime)user.VerificationPinExpiry);
            if(user.VerificationPin == verifyPinRequest.Pin && differenceInMinutes.TotalMinutes < 2)
            {
                user.EmailConfirmed = true;
            }

            if(!user.EmailConfirmed)
            {
                //return BadRequest(error: "Email confirmation failed, Invalid or expired pin");
                return BadRequest(
                    new { message = "Email confirmation failed, Invalid or expired pin" }
                    );
            }

            user.VerificationPin = null;
            user.VerificationPinExpiry = null;

            await _userManager.UpdateAsync(user);
            return Ok(
                new { message = "Email verified successfully" }
                );
        }

        [HttpPost]
        [Route("Re-sendConfirmationMail")]
        public async Task<IActionResult> ResendMail(EmailRequest emailRequest)
        {
            var user = await _userManager.FindByEmailAsync(emailRequest.Email);
            if (user == null)
            {
                return NotFound(new {message = "User not found" });
            }

            if(user.EmailConfirmed)
            {
                return BadRequest(new { message = "Email already veified" });
            }

            // generate new pin with 1 min expiry date
            user.VerificationPin = new Random().Next(100000,999999).ToString();
            user.VerificationPinExpiry = DateTime.UtcNow.AddMinutes(1);

            await _userManager.UpdateAsync(user);

            // send new verification code
            await _emailSender.SendEmailAsync(
                emailRequest.Email,
                "Verification code",
                $"Your verification code is: { user.VerificationPin }"
                );

            return Ok(new {message = "New pin sent to your email." });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginUserRequest loginUserRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginUserRequest.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    return BadRequest(new { message = "Email is not verified. please verify first"});
                }

                var checkPasswordResult = await _userManager.CheckPasswordAsync(user, loginUserRequest.Password);

                if (checkPasswordResult)
                {
                    // get roles
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles != null && roles.Any())
                    {
                        // create token
                        var jwtToken = _tokenRepository.CreateJWTToken(user,roles.ToList());

                        var response = new LoginResponse
                        {
                            JwtToken = jwtToken
                        };
                        return Ok(new {message = response });
                    }
                }
                else
                {
                    return BadRequest(new {message = "Invalid username or password"});
                }
            }

            return NotFound(new { message = "User not found" });
        }

        /// <summary>
        /// Google Login - Accepts user information from frontend after Google authentication
        /// WARNING: This endpoint does not verify the Google token. 
        /// The frontend is responsible for Google authentication security.
        /// For production, consider implementing server-side token verification.
        /// </summary>
        [HttpPost]
        [Route("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // Extract user information from request
                var email = request.Email;
                var name = request.InAppName;
                var googleUserId = request.GoogleUserId;
                var gender = request.Gender;
                var picture = request.ProfilePictureUrl;

                // Validate required fields
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleUserId))
                {
                    return BadRequest(new { message = "Email and Google User ID are required" });
                }

                // Check if user exists
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // NEW USER - Validate required fields for registration
                    if (string.IsNullOrWhiteSpace(gender))
                    {
                        return BadRequest(new { message = "Gender is required for new users" });
                    }

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return BadRequest(new { message = "Name is required for new users" });
                    }

                    // Create new user
                    var newUser = new ApplicationUser
                    {
                        UserName = email.Split('@')[0], // Use email prefix as username
                        Email = email,
                        EmailConfirmed = true, // Google emails are pre-verified
                        InAppName = name,
                        ProfilePictureUrl = picture,
                        LoginProvider = "Google",
                        ExternalProviderUserId = googleUserId,
                        Gender = gender // Use the Gender provided from frontend
                    };

                    // Create user without password
                    var result = await _userManager.CreateAsync(newUser);

                    if (!result.Succeeded)
                    {
                        return BadRequest(new { 
                            message = "Failed to create user account",
                            errors = result.Errors.Select(e => e.Description)
                        });
                    }

                    // Assign default "User" role
                    if (await _roleManager.RoleExistsAsync("User"))
                    {
                        await _userManager.AddToRoleAsync(newUser, "User");
                    }

                    user = newUser;
                }
                else
                {
                    // EXISTING USER - validate login provider
                    if (user.LoginProvider != null && user.LoginProvider != "Google")
                    {
                        return BadRequest(new { 
                            message = $"This email is already registered using {user.LoginProvider} login. Please use {user.LoginProvider} to sign in." 
                        });
                    }

                    // Update login provider if null (for existing users)
                    if (user.LoginProvider == null)
                    {
                        user.LoginProvider = "Google";
                        user.ExternalProviderUserId = googleUserId;
                        await _userManager.UpdateAsync(user);
                    }

                    // Update profile picture if provided and different
                    if (!string.IsNullOrWhiteSpace(picture) && user.ProfilePictureUrl != picture)
                    {
                        user.ProfilePictureUrl = picture;
                        await _userManager.UpdateAsync(user);
                    }

                    // Update InAppName if provided and different
                    if (!string.IsNullOrWhiteSpace(name) && user.InAppName != name)
                    {
                        user.InAppName = name;
                        await _userManager.UpdateAsync(user);
                    }

                    // Update Gender if provided and different
                    if (!string.IsNullOrWhiteSpace(gender) && user.Gender != gender)
                    {
                        user.Gender = gender;
                        await _userManager.UpdateAsync(user);
                    }
                }

                // Generate JWT token
                var roles = await _userManager.GetRolesAsync(user);
                var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                return Ok(new
                {
                    message = new LoginResponse
                    {
                        JwtToken = jwtToken
                    },
                    user = new
                    {
                        email = user.Email,
                        name = user.InAppName,
                        profilePicture = user.ProfilePictureUrl
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Google login");
                return StatusCode(500, new { message = "An error occurred during Google login" });
            }
        }

        [HttpPost]
        [Route("ResetPasswordRequest")]
        public async Task<IActionResult> ResetPasswordRequest(EmailRequest emailRequest)
        {
            var user = await _userManager.FindByEmailAsync(emailRequest.Email);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user" });
            }

            // generate password reset pin
            
            user.PasswordResetPin = new Random().Next(100000,999999).ToString();
            user.PasswordResetPinExpiry = DateTime.UtcNow.AddMinutes(3);

            // send email with pin
            await _emailSender.SendEmailAsync(
                emailRequest.Email,
                "Reset reset",
                $"Pin: {user.PasswordResetPin}"
                );

            await _userManager.UpdateAsync(user);
            return Ok(new { message = "Pin sent to your email" });

        }

        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var timeNow = DateTime.UtcNow;
            var user = await _userManager.FindByEmailAsync (resetPasswordRequest.Email);

            if(user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var differenceInMinutes = timeNow.Subtract((DateTime)user.PasswordResetPinExpiry);

            if(user.PasswordResetPin == resetPasswordRequest.Pin && differenceInMinutes.TotalMinutes < 4)
            {
                // generate password reset token
                var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(
                    user,
                    resetPasswordToken,
                    resetPasswordRequest.NewPassword
                    );

                if(!result.Succeeded)
                {
                    return BadRequest(new { message =result.Errors });
                }

                user.PasswordResetPin = null;
                user.PasswordResetPinExpiry = null;

                await _userManager.UpdateAsync(user);
                return Ok(new {message = "Password has been reset successfully" });
            }

            return BadRequest(new { message = "Invalid or expired pin" });

        }

        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(EmailRequest emailRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailRequest.Email);
                if(user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = await _userManager.DeleteAsync(user);
                if(!result.Succeeded)
                {
                    //return BadRequest(error:result.Errors);
                    return BadRequest(new { message = result.Errors.First().Description });
                }

                return Ok(new {message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                //return BadRequest(error:"internal server error");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] IFormFile? profilePicture, [FromForm] string? inAppName)
        {

            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid Request" });
                }


                if (!string.IsNullOrWhiteSpace(inAppName))
                {
                    user.InAppName = inAppName;
                }

                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var rootPath = _webHostEnvironment.WebRootPath ?? Directory.GetCurrentDirectory();

                    var uploadsPath = Path.Combine(rootPath, "uploads", "profile-pictures");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(profilePicture.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(stream);
                    }

                    user.ProfilePictureUrl = $"/uploads/profile-pictures/{fileName}";
                }
                

                await _userManager.UpdateAsync(user);

                // generate new token
                var roles = await _userManager.GetRolesAsync(user);
                var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

                //return Ok(new { message = "Profile picture uploaded successfully." });
                return Ok(new
                {
                    token = jwtToken
                });
            }
            catch (Exception ex)
            {
                return(StatusCode(500,new {message = "An error occurred",  error = ex.Message}));
            }

        }

        /*[HttpPost("UploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || profilePicture == null || profilePicture.Length == 0)
            {
                return BadRequest(new { message = "Invalid Request" });
            }
           
            
            var rootPath = _webHostEnvironment.WebRootPath ?? Directory.GetCurrentDirectory();

            var uploadsPath = Path.Combine(rootPath, "uploads", "profile-pictures");
            Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(profilePicture.FileName)}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }
            

            user.ProfilePictureUrl = $"/uploads/profile-pictures/{fileName}";
            
            await _userManager.UpdateAsync(user);

            // generate new token
            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

            return Ok(new {
                token = jwtToken
            });
            
        }*/
    }
}
