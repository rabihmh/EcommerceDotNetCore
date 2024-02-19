using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceDotNetCore.Helpers;
using EcommerceDotNetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace EcommerceDotNetCore.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly Jwt _jwt;

    public AuthService(UserManager<ApplicationUser> userManager, IOptions<Jwt> jwt)
    {
        _userManager = userManager;
        _jwt = jwt.Value;
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
     return new AuthModel
     {
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

        authModel.IsAuthenticate = true;
        authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwt);
        authModel.Email = user.Email;
        authModel.Username = user.UserName;
        authModel.ExpiresON = jwt.ValidTo;
        authModel.Roles = rolesList.ToList();

        return authModel;
    }
}