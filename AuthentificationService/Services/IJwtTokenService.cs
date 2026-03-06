using AuthentificationService.Models;

namespace AuthentificationService.Services
{
    public interface IJwtTokenService
    {
        (string token, DateTime expiresAtUtc) CreateAccessToken(User user);
    }
}


// AuthentificationService/Services/IJwtTokenService.cs
//
// Fonctionnalités :
// - Définir un contrat (interface) pour créer un JWT (access token)
// - Retourner à la fois :
//   1) le token (string)
//   2) la date d’expiration (UTC) -> utile côté front pour savoir quand relancer / rafraîchir
//
// Pourquoi une interface ?
// - Pour pouvoir remplacer l’implémentation (tests, autre algo de token, RSA, etc.)
// - Pour respecter le principe DIP (Dependency Inversion) : Controller dépend d’un contrat, pas d’une classe concrète.
