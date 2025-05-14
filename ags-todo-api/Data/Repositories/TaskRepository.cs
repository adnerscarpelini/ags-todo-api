using ags_todo_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ags_todo_api.Data.Repositories
{
    /// <summary>
    /// Implementação concreta da interface <see cref="ITaskRepository"/>.
    /// Esta classe é responsável por encapsular toda a lógica de acesso aos dados
    /// para a entidade <see cref="TaskModel"/>, utilizando o Entity Framework Core
    /// e o <see cref="TodoDbContext"/> para interagir com o banco de dados.
    /// Realiza as operações CRUD (Criar, Ler, Atualizar, Deletar) para as tarefas,
    /// garantindo que estas operações sejam executadas no contexto do usuário
    /// proprietário dos dados.
    /// </summary>
    public class TaskRepository : ITaskRepository
    {
        private readonly TodoDbContext _context;

        public TaskRepository(TodoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona uma nova tarefa ao banco de dados.
        /// Assume que task.UserId já foi definido pelo chamador (Controller).
        /// </summary>
        public async Task AddAsync(TaskModel task)
        {
            // A propriedade CreatedAt e Id são definidas no construtor da TaskModel.
            await _context.Tasks.AddAsync(task);
        }

        /// <summary>
        /// Deleta uma tarefa específica, verificando se pertence ao usuário fornecido.
        /// </summary>
        public async Task DeleteAsync(string taskId, string userId)
        {
            // Busca a tarefa para garantir que ela existe e pertence ao usuário.
            var taskToDelete = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (taskToDelete != null)
            {
                _context.Tasks.Remove(taskToDelete);
            }
        }

        /// <summary>
        /// Obtém todas as tarefas pertencentes a um usuário específico.
        /// </summary>
        public async Task<IEnumerable<TaskModel>> GetAllAsync(string userId)
        {
            return await _context.Tasks
                .Where(t => t.UserId == userId) // Filtra as tarefas pelo UserId
                .ToListAsync();
        }

        /// <summary>
        /// Obtém uma tarefa específica pelo seu ID, verificando se pertence ao usuário fornecido.
        /// </summary>
        public async Task<TaskModel?> GetByIdAsync(string taskId, string userId)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId); // Filtra também pelo UserId
        }

        /// <summary>
        /// Marca uma tarefa como modificada no contexto do EF Core.
        /// O chamador (Controller) é responsável por garantir que a tarefa
        /// pertence ao usuário autenticado antes de chamar este método.
        /// </summary>
        public async Task UpdateAsync(TaskModel task)
        {
            // O EF Core rastreia a entidade e marca seu estado como modificado.
            // Propriedades como UpdatedAt (se existir) devem ser definidas pelo chamador.
            _context.Entry(task).State = EntityState.Modified;
            // O SaveChangesAsync será chamado depois para persistir.
            // Task.CompletedTask é uma forma de retornar uma Task completada quando o método é async mas não tem um await direto aqui.
            // No entanto, como UpdateAsync não precisa retornar nada e pode ser puramente síncrono em sua lógica interna (apenas marcando o estado),
            // você poderia até mesmo torná-lo não-async se quisesse, mas manter async Task é bom para consistência com a interface.
            await Task.CompletedTask; // Para satisfazer o async e não ter warning, já que a operação de marcar estado é síncrona.
                                      // Se você fizesse algo async aqui, como logs, seria diferente.
        }

        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        /// <returns>True se alguma alteração foi salva, False caso contrário.</returns>
        public async Task<bool> SaveChangesAsync()
        {
            // Salva todas as alterações rastreadas (Add, Update, Remove) no banco de dados.
            // Retorna true se pelo menos uma linha foi afetada.
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}