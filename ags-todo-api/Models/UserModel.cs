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

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public UserModel()
        {
            Id = Guid.NewGuid().ToString(); // Gera um novo UUID na criação
        }
    }
}