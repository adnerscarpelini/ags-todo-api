using ags_todo_api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ags_todo_api.Services
{
    /// <summary>
    /// Serviço responsável por gerar tokens JWT para autenticação dos usuários.
    /// </summary>
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key; // Chave de segurança para assinar o token

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            // Busca a chave secreta do appsettings.json e a converte para bytes.
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        }

        /// <summary>
        /// Gera um token JWT para o usuário especificado.
        /// </summary>
        /// <param name="user">O modelo do usuário para o qual o token será gerado.</param>
        /// <returns>Uma string representando o token JWT.</returns>
        public string GenerateToken(UserModel user)
        {
            // 1. Definir os Claims (informações que estarão dentro do token)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // 2. Definir as Credenciais de Assinatura
            // Usar a chave simétrica e o algoritmo HMAC SHA256 para assinar o token.
            // Isso garante para nós que o token não foi modificado e foi realmente gerado por esta API.
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // 3. Definir o Descritor do Token
            // Contém todas as informações para criar o token.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Adiciona os claims ao "assunto" do token
                Expires = DateTime.UtcNow.AddHours(2), // Define o tempo de expiração do token (ex: 2 horas a partir de agora)
                Issuer = _configuration["Jwt:Issuer"], // Emissor do token (configurado no appsettings)
                Audience = _configuration["Jwt:Audience"], // Audiência do token (configurado no appsettings)
                SigningCredentials = creds // Credenciais de assinatura
            };

            // 4. Criar e Escrever o Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // Serializa o token para uma string compacta
        }

        /// <summary>
        /// Retorna a data de expiração que será usada para os tokens gerados.
        /// </summary>
        /// <param name="hoursUntilExpiration">O número de horas até a expiração, a partir de agora.</param>
        /// <returns>A data e hora da expiração do token.</returns>
        public DateTime GetTokenExpiration(int hoursUntilExpiration = 2)
        {
            return DateTime.UtcNow.AddHours(hoursUntilExpiration);
        }
    }
}