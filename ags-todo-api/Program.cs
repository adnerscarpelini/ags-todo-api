using ags_todo_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configurar o DbContext para usar SQLite
// Pega a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//Adner: Configuração do SwaggerGen para gerar o documento swagger.json
builder.Services.AddSwaggerGen(options =>
{
    //Indico que o Sweagger vai ler os comentários XML que eu colocar nos meus métodos do controller
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AGS ToDo API",
        Description = "A simple To-Do List API built with .NET Core and C#.",
        Contact = new OpenApiContact
        {
            Name = "Adner Giovanni Scarpelini",
            Email = "contato@adnerscarpelini.com.br",
            Url = new Uri("https://adnerscarpelini.com.br/")
        },
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // Middleware para servir a UI do Swagger
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AGS ToDo API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
