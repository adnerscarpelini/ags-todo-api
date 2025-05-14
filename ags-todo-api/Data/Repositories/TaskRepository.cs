using ags_todo_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ags_todo_api.Data.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TodoDbContext _context;

        public TaskRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskModel task)
        {
            await _context.Tasks.AddAsync(task);
        }

        public async Task DeleteAsync(int id)
        {
            var taskToDelete = await _context.Tasks.FindAsync(id);
            if (taskToDelete != null)
            {
                _context.Tasks.Remove(taskToDelete);
            }
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskModel?> GetByIdAsync(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }

        public async Task UpdateAsync(TaskModel task)
        {
            _context.Entry(task).State = EntityState.Modified;
        }

        public async Task<bool> SaveChangesAsync()
        {
            // Salva todas as alterações rastreadas (Add, Update, Remove) no banco de dados.
            // Retorna true se pelo menos uma linha foi afetada.
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
