using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PracticeWebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace PracticeWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _context;
        private readonly string _jwtSecretKey;
        private readonly string _jwtIssuer;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, DataContext context, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
            _jwtSecretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
            _jwtIssuer = _configuration.GetValue<string>("JwtSettings:Issuer");
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] UserRegistrationDto model)
        {
            try
            {
                var user = new User { UserName = model.Email, Email = model.Email, Role = "UserRole" };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Message = "Registration failed", Errors = errors });
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                var token = GenerateJwtToken(user);

                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.InnerException?.Message);
                Console.WriteLine(ex.InnerException?.StackTrace);
                return null;
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] UserLoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user);
            return Ok(new { token, role = user.Role, id = user.Id });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(userDto);
        }

        [Authorize(Roles = "AdminRole")]
        [HttpPost("admin")]
        public IActionResult AdminAction()
        {
            // Validate the JWT token
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecretKey = _configuration["JwtSettings:SecretKey"];

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtIssuer,
                    ValidateAudience = false, // If the token doesn't have an audience claim
                                              // Additional validation options like validating the token expiration can be set here
                }, out var validatedToken);

                // Token is valid, continue with the admin action
                Console.WriteLine("Admin action successful.");
                return Ok(new { message = "Admin action successful." });
            }
            catch (Exception ex)
            {
                // Token validation failed
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Unauthorized();
            }
        }

        private string GenerateJwtToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey);

            try
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? string.Empty),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _jwtIssuer
                };


                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                Console.WriteLine("Token generated successfully: " + tokenString);

                return tokenString;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating token: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }
    }
}
