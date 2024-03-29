using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    // service need to be injected
    public class TokenService
    {
        // inject iConfiguration to use environment variables
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }
        public string CreateToken(AppUser user)
        {
            // the payload of JWT
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };

            // sign token with a secret key stored on server (stored in appsettings.Development.json)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
            // credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // prepare token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // pass in claims (payload)
                Subject = new ClaimsIdentity(claims),
                // auto expire token after 7 days
                Expires = DateTime.Now.AddDays(7),
                // pass in signing credentials
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // create token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}