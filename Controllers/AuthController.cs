using EcommerceDotNetCore.Models;
using EcommerceDotNetCore.Services.Auth;
using EcommerceDotNetCore.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        [HttpPost]

        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result =await _authService.RegisterAsync(model);
            if(result.IsAuthenticate is false)
                return BadRequest(result.Message);
            //return Ok(new { token = result.Token, expireOn = result.ExpiresON });
            return Ok(result);
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> GetTokenAsync([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _authService.GetTokenAsync(model);
            if (result.IsAuthenticate is false)
                return BadRequest(result.Message);
            return Ok(result);
        }  
        [HttpGet("sendTestEmail/{to}")]
        public async Task<IActionResult> SendTestEmail(string to)
        {
            await _emailService.SendEmailAsync(to, "Test Email", "<h1>Test Email</h1>");
            return Ok("Email Sent");
        }
        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailModel model)
        {
            var result = await _authService.ConfirmEmail(model.UserId, model.Token);
            if (result.IsEmailConfirm is false)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }
       
    }
}
