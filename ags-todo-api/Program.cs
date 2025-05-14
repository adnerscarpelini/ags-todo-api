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
// Pega a string de conex�o do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlite(connectionString));


//Adner: Configura��o de Inje��o de Depend�ncia para o reposit�rio de tarefas e Token de autentica��o
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<TokenService>();

// === IN�CIO DA CONFIGURA��O DE AUTENTICA��O JWT ===
builder.Services.AddAuthentication(options =>
{
    // Define o esquema de autentica��o padr�o como JWT Bearer.
    // Isso significa que, por padr�o, a API tentar� autenticar usando tokens JWT.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Configura o manipulador de token JWT Bearer
{
    options.SaveToken = true; // Opcional: Salva o token no HttpContext ap�s a valida��o bem-sucedida
    options.RequireHttpsMetadata = false; // Em desenvolvimento, pode ser false. Em produ��o, idealmente true.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Valida a chave de assinatura do token (para garantir que foi assinado pela sua API)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

        // Valida o emissor do token (quem gerou o token)
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // Valida a audi�ncia do token (para quem o token foi destinado)
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Valida o tempo de vida do token (se n�o expirou)
        ValidateLifetime = true,

    };
});
builder.Services.AddAuthorization();
// === FIM DA CONFIGURA��O DE AUTENTICA��O JWT ===


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


//Adner: Configura��o do SwaggerGen para gerar o documento swagger.json
builder.Services.AddSwaggerGen(options =>
{
    //Indico que o Sweagger vai ler os coment�rios XML que eu colocar nos meus m�todos do controller
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

    // Configura��o para habilitar a autoriza��o JWT no Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, // O token ser� enviado no cabe�alho da requisi��o
        Description = "Por favor, insira 'Bearer' seguido de um espa�o e o seu token JWT. Exemplo: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",        // Nome do cabe�alho (Authorization)
        Type = SecuritySchemeType.Http, // Tipo do esquema de seguran�a
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

app.UseAuthentication(); // 1�: O sistema tenta autenticar o usu�rio (ex: validar o token JWT)
                         // e preenche o HttpContext.User com a identidade do usu�rio.

app.UseAuthorization();  // 2�: O sistema verifica se o usu�rio (agora autenticado)
                         // tem permiss�o para acessar o recurso solicitado (baseado em [Authorize], roles, etc.).

app.MapControllers();

app.Run();
