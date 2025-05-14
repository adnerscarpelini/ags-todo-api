using ags_todo_api.Models;

namespace ags_todo_api.Data.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskModel?> GetByIdAsync(int id);
        Task<IEnumerable<TaskModel>> GetAllAsync();
        Task AddAsync(TaskModel task);
        Task UpdateAsync(TaskModel task);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync(); // Para confirmar as operações no banco
    }
}
