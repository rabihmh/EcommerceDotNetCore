﻿using EcommerceDotNetCore.Models;
using EcommerceDotNetCore.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceDotNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
            return Ok(result);

        }
    }
}
