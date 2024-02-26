using EcommerceDotNetCore.Models;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceDotNetCore.Services.Auth;

public interface IAuthService
{
    public Task<AuthModel> RegisterAsync(RegisterModel model);
    public  Task<JwtSecurityToken> CreateJwtToken(ApplicationUser applicationUser);
    public Task<AuthModel> GetTokenAsync(TokenRequestModel model);
    public Task<AuthModel> GetUserDetails();
    public Task<AuthModel> ConfirmEmail(string userId, string token);
    //public Task<string>AddRoleModelAsync(AddRoleModel model);
}