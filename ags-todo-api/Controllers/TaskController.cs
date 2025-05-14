using ags_todo_api.Data.Repositories;
using ags_todo_api.DTOs;
using ags_todo_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ags_todo_api.Controllers
{
    /// <summary>
    /// Controller para gerenciar as tarefas (To-Do items) dos usuários.
    /// Todas as operações requerem autenticação.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        // Método de Apoio para obter o ID do usuário logado a partir dos claims do token JWT
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Obtém todas as tarefas do usuário autenticado.
        /// </summary>
        /// <returns>Uma lista de tarefas do usuário.</returns>
        /// <response code="200">Retorna a lista de tarefas.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetAllTasksForUser()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não identificado.");

            var tasks = await _taskRepository.GetAllAsync(userId);
            return Ok(tasks);
        }

        /// <summary>
        /// Obtém uma tarefa específica do usuário autenticado pelo seu ID.
        /// </summary>
        /// <param name="taskId">O ID (UUID) da tarefa a ser obtida.</param>
        /// <returns>A tarefa encontrada.</returns>
        /// <response code="200">Retorna a tarefa solicitada.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="404">Se a tarefa não for encontrada ou não pertencer ao usuário.</response>
        [HttpGet("{taskId}")]
        [ProducesResponseType(typeof(TaskModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskModel>> GetTaskByIdForUser(string taskId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não identificado.");

            var task = await _taskRepository.GetByIdAsync(taskId, userId);

            if (task == null)
                return NotFound("Tarefa não encontrada ou não pertence ao usuário.");

            return Ok(task);
        }

        /// <summary>
        /// Cria uma nova tarefa para o usuário autenticado.
        /// </summary>
        /// <param name="taskCreateDto">Os dados para a nova tarefa.</param>
        /// <returns>A tarefa recém-criada.</returns>
        /// <response code="201">Retorna a tarefa recém-criada.</response>
        /// <response code="400">Se os dados da tarefa forem inválidos.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TaskModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TaskModel>> CreateTaskForUser([FromBody] TaskCreateDto taskCreateDto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não identificado.");

            if (!ModelState.IsValid) // Validação automática pelo [ApiController] e DataAnnotations no DTO
                return BadRequest(ModelState);

            var task = new TaskModel // Mapeamento do DTO para o Model
            {
                // Id e CreatedAt são definidos no construtor da TaskModel
                Title = taskCreateDto.Title,
                Description = taskCreateDto.Description,
                IsCompleted = false,
                DueDate = taskCreateDto.DueDate,
                UserId = userId
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync(); // Salva as alterações no banco

            return CreatedAtAction(nameof(GetTaskByIdForUser), new { taskId = task.Id }, task);
        }

        /// <summary>
        /// Atualiza uma tarefa existente do usuário autenticado.
        /// </summary>
        /// <param name="taskId">O ID (UUID) da tarefa a ser atualizada.</param>
        /// <param name="taskUpdateDto">Os dados com as atualizações para a tarefa.</param>
        /// <returns>Nenhum conteúdo em caso de sucesso.</returns>
        /// <response code="204">Se a tarefa foi atualizada com sucesso.</response>
        /// <response code="400">Se os dados da tarefa forem inválidos.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="404">Se a tarefa não for encontrada ou não pertencer ao usuário.</response>
        [HttpPut("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTaskForUser(string taskId, [FromBody] TaskUpdateDto taskUpdateDto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Usuário não identificado.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingTask = await _taskRepository.GetByIdAsync(taskId, userId);
            if (existingTask == null)
                return NotFound("Tarefa não encontrada ou não pertence ao usuário.");

            if (taskUpdateDto.Title != null)
                existingTask.Title = taskUpdateDto.Title;

            if (taskUpdateDto.Description != null)
                existingTask.Description = taskUpdateDto.Description;
            
            if (taskUpdateDto.IsCompleted.HasValue)
                existingTask.IsCompleted = taskUpdateDto.IsCompleted.Value;

            if (taskUpdateDto.DueDate.HasValue) // Permite limpar a DueDate se null for passado
                existingTask.DueDate = taskUpdateDto.DueDate;

            await _taskRepository.UpdateAsync(existingTask);
            await _taskRepository.SaveChangesAsync();

            return NoContent(); // Padrão para PUT bem-sucedido que não retorna o corpo
        }

        /// <summary>
        /// Deleta uma tarefa específica do usuário autenticado.
        /// </summary>
        /// <param name="taskId">O ID (UUID) da tarefa a ser deletada.</param>
        /// <returns>Nenhum conteúdo em caso de sucesso.</returns>
        /// <response code="204">Se a tarefa foi deletada com sucesso.</response>
        /// <response code="401">Se o usuário não estiver autenticado.</response>
        /// <response code="404">Se a tarefa não for encontrada ou não pertencer ao usuário.</response>
        [HttpDelete("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTaskForUser(string taskId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não identificado.");
            }

            // A lógica de DeleteAsync no repositório já verifica se a tarefa pertence ao usuário
            // antes de tentar deletar, então não precisamos chamar GetByIdAsync aqui explicitamente
            // para verificar posse, mas sim para saber se algo foi de fato deletado para o SaveChangesAsync.
            var taskToDelete = await _taskRepository.GetByIdAsync(taskId, userId);
            if (taskToDelete == null)
                return NotFound("Tarefa não encontrada ou não pertence ao usuário.");

            await _taskRepository.DeleteAsync(taskId, userId);
            await _taskRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}