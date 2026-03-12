namespace AuthentificationService.Dtos
{
    /// <summary>
    /// Retourné après une demande de réinitialisation de mot de passe.
    /// En production, le token serait envoyé par e-mail uniquement.
    /// Ici, il est retourné directement dans la réponse pour faciliter les tests.
    /// </summary>
    public class ForgotPasswordResultDto
    {
        public string Message { get; set; } = string.Empty;

        /// <summary>Token à utiliser avec POST /api/auth/reset-password</summary>
        public string ResetToken { get; set; } = string.Empty;

        /// <summary>Date d'expiration du token (1 heure)</summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}
