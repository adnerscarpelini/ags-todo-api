// Em ags-todo-api/DTOs/TaskUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace ags_todo_api.DTOs
{
    /// <summary>
    /// DTO para atualizar uma tarefa existente.
    /// Todos os campos são opcionais; apenas os fornecidos serão atualizados.
    /// </summary>
    public class TaskUpdateDto
    {
        [StringLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
        public string? Title { get; set; }

        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres.")]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }
    }
}