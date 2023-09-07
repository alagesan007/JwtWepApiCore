using JwtWepApiCore.Models.Authentication;
using JwtWepApiCore.Models.Authentication.Login;
using JwtWepApiCore.Models.Authentication.Signup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NETCore.MailKit.Core;
using Org.BouncyCastle.Asn1.Cms;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Management.Service.Models;
using User.Management.Service.Services;

namespace JwtWepApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMyEmailService _emailServce;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IMyEmailService emailServce, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailServce = emailServce;
            _configuration = configuration;
        }
       

        [HttpPost]
        public async Task<ActionResult> Register([FromBody] Registration registerUser, string role)
        {
            //Check The User Already Exist
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseMessage { Status = "ERROR", Message = "User Already Exist!" });
            }

            //Add The New User

            IdentityUser user = new()
            {
                Email = registerUser.Email,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.UserName
            };

            //Check The User Role Exist or Not 
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.CreateAsync(user, registerUser.Password);
                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseMessage { Status = "Error", Message = "User Failed to Create!" });
                }

                // Add The New User 

                await _userManager.AddToRoleAsync(user, role);

                // Add token to verify the user
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action(nameof(confirmEmail), "Authentication", new { token, email = user.Email });
                var message = new Message(new string[] { user.Email }, "ConfirmationEmailLink", confirmationLink);

                //_emailServce.SendEmail(message);

                return StatusCode(StatusCodes.Status201Created,
                    new ResponseMessage { Status = "Success", Message = $"User Created Succesfully and its Confirmation Link - {confirmationLink}" });


            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseMessage { Status = "Error", Message = "User Role Does Not Exists!" });
            }
        }

        [HttpGet]
        public ActionResult EmailSending()
        {
            var message = new Message(new string[] { "savanna54@ethereal.email" }, "Test", "<h1>Testing my by alagesh</h1>");

            _emailServce.SendEmail(message);
            return StatusCode(StatusCodes.Status201Created,
                    new ResponseMessage { Status = "Success", Message = "Email Send Successfully!" });
        }

        [HttpGet("ConfirmEmail")]

        public async Task<IActionResult> confirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status201Created,
                    new ResponseMessage { Status = "Success", Message = "Email Verified Successfully!" });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                    new ResponseMessage { Status = "Error", Message = "This User Does Not Exists!" });
        }

        private JwtSecurityToken getToken(List<Claim> authClaim)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:ValidIssuer"],
                audience: _configuration["Jwt:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: authClaim,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] Login loginModel)
        {
            //checking the user exist or not
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if(user != null && await _userManager.CheckPasswordAsync(user,loginModel.Password))
            {
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name , user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var userRols = await _userManager.GetRolesAsync(user);
                foreach (var role in userRols)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var jwtToken = getToken(authClaims);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });
            }

            return Unauthorized();
            //checking tha password

            //generate a claimlist

            //adds role to the list

            //generate the token with the claims..

            //retruning the token



            
        }

        
    }
}
