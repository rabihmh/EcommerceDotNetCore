using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceDotNetCore.Configurations;
using EcommerceDotNetCore.Models;
using EcommerceDotNetCore.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace EcommerceDotNetCore.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Jwt _jwt;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<ApplicationUser> userManager,
                       IOptions<Jwt> jwt,
                       IHttpContextAccessor httpContextAccessor,
                       IEmailService emailService,
                       ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<AuthModel> RegisterAsync(RegisterModel model)
    {
        if (await _userManager.FindByEmailAsync(model.Email) is not null)
            return new AuthModel() { Message = "Email Is Already Register" };

        if (await _userManager.FindByNameAsync(model.UserName) is not null)
            return new AuthModel() { Message = "Username Is Already Register" };
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };
     var result=  await _userManager.CreateAsync(user, model.Password);
     if (!result.Succeeded)
     {
         var errors = string.Empty;
         foreach (var error in result.Errors)
         {
             errors += $"{error.Description},";
         }
         return new AuthModel { Message = errors };
     }

     await _userManager.AddToRoleAsync(user,"User");
     var jwtToken = await CreateJwtToken(user);
     await SendConfirmationEmail(user);
     return new AuthModel
     {
         Message = "Registered successfully",
         Email = user.Email,
         ExpiresON = jwtToken.ValidTo,
         IsAuthenticate = true,
         Roles = new List<string>{"User"},
         Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
         Username = user.UserName
     };
    }

    public async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser applicationUser)
    {
        var userClaims = await _userManager.GetClaimsAsync(applicationUser);
        var roles = await _userManager.GetRolesAsync(applicationUser);
        var roleClaims = new List<Claim>();
        foreach (var role in roles)
            roleClaims.Add(new Claim("roles",role));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
            new Claim("uid",applicationUser.Id)
        }.Union(userClaims)
         .Union(roleClaims);
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var signinglCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            signingCredentials: signinglCredentials,
            expires: DateTime.Now.AddDays(_jwt.DurationInDays));

        return jwtSecurityToken;
    }

    public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
    {
        var authModel=new AuthModel();
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            authModel.Message = "Email or Password is incorrect";
            return authModel;
        }

        
        var jwt = await CreateJwtToken(user);
        var rolesList = await _userManager.GetRolesAsync(user);
        authModel.Message = "success";
        authModel.IsAuthenticate = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
        authModel.Email = user.Email;
        authModel.Username = user.UserName;
        authModel.ExpiresON = jwt.ValidTo;
        authModel.Roles = rolesList.ToList();

        return authModel;
    }

    public async Task<AuthModel> GetUserDetails()
    {
        
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity;
            var emailClaim = ((ClaimsIdentity)claimsIdentity).FindFirst(ClaimTypes.Email)?.Value;

            if (emailClaim != null)
            {
                var user = await _userManager.FindByEmailAsync(emailClaim);
                if (user != null)
                {


                    return new AuthModel
                    {
                        Email = user.Email,
                        IsAuthenticate = true,
                        Roles = new List<string> { "User" },
                        Username = user.UserName
                    };
                }
            }
            return null;
    }
    public async Task<AuthModel> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return new AuthModel { Message = "User Not Found" };
        var result = await _userManager.ConfirmEmailAsync(user, token);
        _logger.LogInformation(JObject.FromObject(result).ToString());
        if (result.Succeeded)
            return new AuthModel { Message = "Email Confirmed Successfully", IsEmailConfirm = true};
        return new AuthModel { Message = "Email Confirmation Failed" };
    }
    private async Task SendConfirmationEmail(ApplicationUser user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _httpContextAccessor.HttpContext.Request.Host + $"/api/Auth/confirmemail/{user.Id}/{token}";
        var message = $@"
        <h1>Please Verify Your Email</h1>
        Hello {user.UserName},<br>
        Please click the below link to verify your email<br>
        <a href='{confirmationLink}' style='display: inline-block; padding: 10px 20px; background-color: #007bff; color: #ffffff; text-decoration: none;'>Click Here</a><br>
        If the above link does not work, you can copy and paste the below link in your browser<br>
        {confirmationLink}<br>
        <a href='{confirmationLink}'>{confirmationLink}</a><br>
        userId:<strong>{user.Id}</strong><br>
        token:<strong>{token}</strong><br>
        If you have not registered, please ignore this email
    ";
        _logger.LogInformation("id: " + user.Id);
        _logger.LogInformation("Token: " + token);
        _logger.LogInformation("Confirmation Link: " + confirmationLink);
        await _emailService.SendEmailAsync(user.Email, "Confirm Email", message);
    }

}