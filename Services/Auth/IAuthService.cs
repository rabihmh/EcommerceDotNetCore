using EcommerceDotNetCore.Models;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceDotNetCore.Services.Auth;

public interface IAuthService
{
    public Task<AuthModel> RegisterAsync(RegisterModel model);
    public  Task<JwtSecurityToken> CreateJwtToken(ApplicationUser applicationUser);
}