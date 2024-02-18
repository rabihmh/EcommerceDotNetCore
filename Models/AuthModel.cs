﻿namespace EcommerceDotNetCore.Models
{
    public class AuthModel
    {
        public string Message { get; set; }

        public bool IsAuthenticate { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresON { get; set; }
    }
}
