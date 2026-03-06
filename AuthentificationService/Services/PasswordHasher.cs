using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace AuthentificationService.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        // Hash(password) :
        // - Génère un sel (salt) aléatoire
        // - Dérive un hash PBKDF2 à partir du mot de passe + sel
        // - Retourne une chaîne "salt.hash" en Base64 (facile à stocker en DB)
        public string Hash(string password)
        {
            // 1) Génération d'un sel cryptographiquement sécurisé (16 bytes = 128 bits)
            //    Chaque utilisateur aura un sel différent, même si deux personnes ont le même mot de passe.
            byte[] salt = RandomNumberGenerator.GetBytes(16);


            // 2) Dérivation d'une clé (hash) via PBKDF2 :
            //    - HMACSHA256 comme fonction pseudo-aléatoire
            //    - 100 000 itérations : ralentit le brute-force
            //    - 32 bytes demandés : taille du hash final
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32);

            // 3) On stocke "salt.hash" en Base64 :
            //    Ex: "pQ2...==.0xA...=="
            //    Le salt est nécessaire pour vérifier plus tard.
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        // Verify(password, passwordHash) :
        // - Récupère le salt depuis la DB (première partie)
        // - Recalcule le hash avec le même salt et les mêmes paramètres PBKDF2
        // - Compare le hash recalculé au hash stocké
        public bool Verify(string password, string passwordHash)
        {
            // 1) On s'attend à un format "salt.hash"
            var parts = passwordHash.Split('.', 2);
            if (parts.Length != 2) return false;

            // 2) Décodage Base64 : récupération du salt et du hash attendu
            var salt = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            // 3) Recalcul du hash avec le salt d'origine
            byte[] actualHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100_000,
                numBytesRequested: 32);

            // 4) Comparaison en temps constant 
            //    Evite de fuiter des infos par le temps de comparaison.
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
    }
}

// AuthentificationService/Services/PasswordHasher.cs
//
// Fonctionnalités de ce composant :
// 1) HASHER un mot de passe en le transformant en une valeur non réversible (sécurité)
// 2) STOCKER le hash sous forme "salt.hash" (2 parties Base64 séparées par un point)
// 3) VÉRIFIER un mot de passe : recalculer le hash avec le même salt et comparer en temps constant
//
//
// Algo utilisé : PBKDF2 (Password-Based Key Derivation Function 2)
// - PRF: HMACSHA256
// - Iterations: 100 000 (plus c’est élevé, plus c’est lent pour un attaquant)
// - Hash length: 32 bytes (256 bits)