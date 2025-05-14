using System.ComponentModel.DataAnnotations;

namespace ags_todo_api.Models
{
    public class TaskModel
    {
        [Key] // Usa o "Id" como chave
        public int Id { get; set; }

        [Required(ErrorMessage = "O título da tarefa é obrigatório.")]
        [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? DueDate { get; set; }
    }
}