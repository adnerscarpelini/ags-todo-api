using System.ComponentModel.DataAnnotations;

namespace ags_todo_api.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) utilizado para transportar os dados necessários
    /// para o registro de um novo usuário na aplicação.
    /// Contém as credenciais fornecidas pelo cliente para a criação da conta.
    /// </summary>
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 50 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        // Você pode adicionar mais validações de senha se desejar, como exigir caracteres especiais, números, etc.
        // usando Regular Expressions com o atributo [RegularExpression("seu_regex_aqui")]
        public string Password { get; set; } = string.Empty;
    }
}
