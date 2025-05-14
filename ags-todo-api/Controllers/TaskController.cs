using ags_todo_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ags_todo_api.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Obtém uma tarefa específica pelo seu ID.
        /// </summary>
        /// <param name="id">O ID da tarefa a ser obtida.</param>
        /// <returns>A tarefa encontrada ou NotFound se não existir.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskModel>> GetTask(int id)
        {
            return new TaskModel();
        }
    }
}
