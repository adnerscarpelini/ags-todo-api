using ags_todo_api.Models;
using Microsoft.EntityFrameworkCore;

namespace ags_todo_api.Data
{
    public class TodoDbContext : DbContext
    {
        // Construtor que recebe as opções de configuração do DbContext
        // Essas opções são injetadas pelo sistema de injeção de dependência
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        // Aqui, estamos dizendo ao EF Core que queremos uma tabela para nossas TaskModels
        public DbSet<TaskModel> Tasks { get; set; }
    }
}