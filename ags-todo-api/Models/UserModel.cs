using System.ComponentModel.DataAnnotations;

namespace ags_todo_api.Models
{
    public class UserModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        // As senhas NUNCA devem ser armazenadss como string pura!
        // Então fiz isso para armazenar a senha de forma segura usando um
        //algoritmo de hashing como o BCrypt.
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public UserModel()
        {
            Id = Guid.NewGuid().ToString(); // Gera um novo UUID na criação
        }
    }
}