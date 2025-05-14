using System.ComponentModel.DataAnnotations;

namespace ags_todo_api.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) utilizado para transportar os dados necessários
    /// para o login de um usuário existente na aplicação.
    /// Contém as credenciais fornecidas pelo cliente para autenticação.
    /// </summary>
    public class UserLoginDto
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; } = string.Empty;
    }
}
