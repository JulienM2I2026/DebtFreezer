using System.ComponentModel.DataAnnotations;

namespace AuthentificationService.Models
{
    public class User
    {

        public Guid UserId { get; set; } = Guid.NewGuid(); //Permet d'obtenir un identifiant unique sur 128 bits

        [Required, EmailAddress, StringLength(320)]
        public string Email { get; set; } = string.Empty;

        
        [Required, StringLength(120)]
        public string FullName { get; set; } = string.Empty;

        //public int TotalDebt { get; set; } A récupérer je pense via API DB Debt

        // Stocke un hash (jamais le mot de passe)
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Constructeur vide (utile pour EF + object initializer)
        public User() { }

        // Constructeur pratique
        public User(string email, string fullName, string passwordHash)
        {
            Email = email;
            FullName = fullName;
            //TotalDebt = totalDebt;
            PasswordHash = passwordHash;    
        }
    } 
}