using AuthentificationService.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthentificationService.Services
{

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        public JwtTokenService(IConfiguration config) => _config = config;

        public (string token, DateTime expiresAtUtc) CreateAccessToken(User user)
        {
            // 1) Lecture de la configuration JWT
            var issuer = _config["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer missing");
            var audience = _config["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience missing");
            var key = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key missing");

            // 2) Durée de validité
            var expiresMinutesStr = _config["Jwt:ExpiresMinutes"] ?? "60";
            if (!int.TryParse(expiresMinutesStr, out var expiresMinutes)) expiresMinutes = 60;

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

            // 3) Claims : données encodées dans le token
            // - sub (standard) : identifiant unique du sujet (ici userId)
            // - email (standard) : email
            // - fullName (custom) : info utile côté UI / logs (facultatif)
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("fullName", user.FullName)
        };

            // 4) Signature : clé symétrique HMAC-SHA256
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 5) Construction du JWT
            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds);

            // 6) Sérialisation en string
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return (token, expiresAtUtc);
        }
    }
}

// AuthentificationService/Services/JwtTokenService.cs
//
// Fonctionnalités :
// - Construire un JWT signé (HMAC-SHA256) à partir d’un User
// - Injecter des "claims" dans le token :
//     - sub : identifiant utilisateur (Guid)
//     - email : email utilisateur
//     - fullName : nom complet (custom claim)
// - Gérer l’expiration en minutes (Jwt:ExpiresMinutes)
// - Lire la config depuis appsettings.json (Issuer, Audience, Key)
//
// Rappels JWT :
// - Issuer = qui émet le token (ton AuthentificationService)
// - Audience = qui est censé accepter le token (ex: "DebtFreezer" ou "Gateway")
// - Key = secret symétrique utilisé pour signer. Doit être long et stocké en secret manager en prod
//