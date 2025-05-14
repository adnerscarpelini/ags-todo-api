using System.ComponentModel.DataAnnotations;

namespace ags_todo_api.Models
{
    public class TaskModel
    {
        [Key] // Usa o "Id" como chave
        public string Id { get; set; }

        [Required(ErrorMessage = "O título da tarefa é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; private set; } //Privado, pois já estou iniciando no construtor!

        public DateTime? DueDate { get; set; }

        // Relacionamento com UserModel
        [Required] 
        public string UserId { get; set; } = string.Empty;
        public UserModel User { get; set; } = null!;


        public TaskModel()
        {
            Id = Guid.NewGuid().ToString(); // Gera um novo UUID na criação
            CreatedAt = DateTime.UtcNow;
        }
    }
}