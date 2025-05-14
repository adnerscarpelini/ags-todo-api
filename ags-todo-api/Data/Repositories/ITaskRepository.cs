using ags_todo_api.Models;

namespace ags_todo_api.Data.Repositories
{
    /// <summary>
    /// Define o contrato (a interface) para as operações de acesso a dados relacionadas às Tarefas (Tasks).
    /// Esta interface abstrai a lógica de persistência de dados das tarefas, permitindo que
    /// a camada de negócios (Controllers, Services) interaja com os dados das tarefas
    /// de forma desacoplada da implementação concreta do acesso ao banco de dados.
    /// Facilita a injeção de dependência e a testabilidade da aplicação (ex: usando mocks).
    /// Todas as operações devem considerar o contexto do usuário (userId) para garantir
    /// que um usuário só possa acessar e manipular suas próprias tarefas.
    /// </summary>
    public interface ITaskRepository
    {
        Task<TaskModel?> GetByIdAsync(string taskId, string userId);
        Task<IEnumerable<TaskModel>> GetAllAsync(string userId);
        Task AddAsync(TaskModel task);
        Task UpdateAsync(TaskModel task); // O controller deve garantir que a task pertence ao usuário
        Task DeleteAsync(string taskId, string userId);
        Task<bool> SaveChangesAsync();
    }
}