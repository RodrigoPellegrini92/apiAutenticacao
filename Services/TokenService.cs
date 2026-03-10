using apiAutenticacao.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace apiAutenticacao.Services
{
    public class TokenService
    {
       private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration) 
        { 
            _configuration = configuration;
        }

        public string GenerateToken(Usuario user) {

            string key = _configuration["JWT:"]!;
            string issuer = _configuration["JWT:Issuer"]!;
            string audience = _configuration["JWT:Audience"]!;
            int durationInHours = int.Parse(_configuration["JWT:ExpireAt"]!);
           

            var bytesKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(bytesKey, SecurityAlgorithms.HmacSha256);

            string id = Guid.NewGuid().ToString();

            var claims = new[]{
             new Claim(ClaimTypes.Email, user.Email ),
             new Claim(ClaimTypes.Name,user.Nome),
         
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(durationInHours),
                signingCredentials: credentials
                );

            //Converte o objeto token para string que sera enviada para o cliente
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
   

        }

    }

