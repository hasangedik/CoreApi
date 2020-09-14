using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoreApi.Contract.DatabaseContracts;
using Microsoft.IdentityModel.Tokens;

namespace CoreApi.WebApi.Auth
{
    public static class JwtManager
    {
        private static readonly byte[] SymmetricKey = Convert.FromBase64String("GZ1E3szROm8VqQX43S53bfk11Cx74FPlRYbq6nZtW+tcbCoMnT1nsBMRa1Av1DZ2mTkrfx+PoAJjQ1LxflDmAw==");

        public static TokenValidationParameters ValidationParameters => new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(SymmetricKey)
        };

        public static JwtSecurityToken GenerateToken(UserContract user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(SymmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };
            return (JwtSecurityToken)tokenHandler.CreateToken(tokenDescriptor);
        }
    }
}
