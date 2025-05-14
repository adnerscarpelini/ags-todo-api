namespace ags_todo_api.DTOs
{
    /// <summary>
    /// Data Transfer Object (DTO) utilizado para retornar a resposta ao cliente
    /// após um processo de login bem-sucedido.
    /// Contém o token de autenticação e outras informações relevantes do usuário.
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime Expiration { get; set; } // Para informar ao cliente quando o token expira
    }
}
