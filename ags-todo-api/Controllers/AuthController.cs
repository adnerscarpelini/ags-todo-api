using ags_todo_api.Data;
using ags_todo_api.DTOs;
using ags_todo_api.Models;
using ags_todo_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ags_todo_api.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints de autenticação (registro e login).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TodoDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(TodoDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registra um novo usuário na aplicação.
        /// </summary>
        /// <param name="registerDto">Dados para o registro do usuário.</param>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Nome de usuário já existente. Por favor, escolha outro.");
            }

            // Gera o hash da senha usando BCrypt
            string passwordHash = BCryptNet.HashPassword(registerDto.Password);

            var user = new UserModel
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Por questão de segurança, eu não retorno o ID do usuário ou o hash da senha, apenas uma mensagem de sucesso.
            return Ok(new { Message = "Usuário registrado com sucesso!" });
        }


        /// <summary>
        /// Autentica um usuário existente e retorna um token JWT.
        /// </summary>
        /// <param name="loginDto">Credenciais de login do usuário.</param>
        /// <returns>Um token JWT e informações do usuário em caso de sucesso.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login(UserLoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return Unauthorized("Nome de usuário ou senha inválidos.");
            }

            // Verifica a senha usando BCrypt
            if (!BCryptNet.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Nome de usuário ou senha inválidos.");
            }

            var token = _tokenService.GenerateToken(user);
            var tokenExpiration = _tokenService.GetTokenExpiration();

            return Ok(new LoginResponseDto
            {
                Token = token,
                Username = user.Username,
                Expiration = tokenExpiration
            });
        }

        /// <summary>
        /// Testa se a Autenticação foi realizada com sucesso
        /// </summary>
        [HttpGet("testauth")]
        [Microsoft.AspNetCore.Authorization.Authorize] // Protege este endpoint
        public IActionResult TestAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Pega o ID do usuário do token
            var username = User.FindFirstValue(ClaimTypes.Name); // Pega o nome do usuário do token
            return Ok(new { Message = $"Autenticado com sucesso! UserID: {userId}, Username: {username}" });
        }
    }
}