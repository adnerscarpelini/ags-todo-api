using ags_todo_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ags_todo_api.Services;
using ags_todo_api.Data.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Adner: Configurar o DbContext para usar SQLite
// Pega a string de conexão do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(connectionString));


//Adner: Configuração de Injeção de Dependência para o repositório de tarefas e Token de autenticação
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TokenService>();

// === INÍCIO DA CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ===
builder.Services.AddAuthentication(options =>
{
    // Define o esquema de autenticação padrão como JWT Bearer.
    // Isso significa que, por padrão, a API tentará autenticar usando tokens JWT.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Configura o manipulador de token JWT Bearer
{
    options.SaveToken = true; // Opcional: Salva o token no HttpContext após a validação bem-sucedida
    options.RequireHttpsMetadata = false; // Em desenvolvimento, pode ser false. Em produção, idealmente true.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida a chave de assinatura do token (para garantir que foi assinado pela sua API)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

        // Valida o emissor do token (quem gerou o token)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // Valida a audiência do token (para quem o token foi destinado)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Valida o tempo de vida do token (se não expirou)
        ValidateLifetime = true,

    };
});
builder.Services.AddAuthorization();
// === FIM DA CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ===


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

    // Configuração para habilitar a autorização JWT no Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, // O token será enviado no cabeçalho da requisição
        Description = "Por favor, insira 'Bearer' seguido de um espaço e o seu token JWT. Exemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",        // Nome do cabeçalho (Authorization)
        Type = SecuritySchemeType.Http, // Tipo do esquema de segurança
        Scheme = "Bearer",              // Esquema usado (Bearer)
        BearerFormat = "JWT"            // Formato do token
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
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

app.UseAuthentication(); // 1º: O sistema tenta autenticar o usuário (ex: validar o token JWT)
                         // e preenche o HttpContext.User com a identidade do usuário.

app.UseAuthorization();  // 2º: O sistema verifica se o usuário (agora autenticado)
                         // tem permissão para acessar o recurso solicitado (baseado em [Authorize], roles, etc.).

app.MapControllers();

app.Run();
