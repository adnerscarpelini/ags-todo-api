/*
 * =================================================================================================
 * TodoDbContext - Contexto do Banco de Dados da Aplicação (ags-todo-api)
 * =================================================================================================
 * Esta classe é o coração da interação com o banco de dados usando o Entity Framework Core (EF Core).
 * Ela é responsável por:
 *
 * 1. REPRESENTAR A SESSÃO COM O BANCO: Cada instância desta classe representa uma sessão
 * com o banco de dados, permitindo consultar e salvar dados.
 *
 * 2. DEFINIR AS ENTIDADES (TABELAS): As propriedades do tipo DbSet<TEntity> (como Tasks e Users)
 * mapeiam para as tabelas no banco de dados.
 *
 * 3. CONFIGURAR O MODELO DE DADOS: O método OnModelCreating é usado para configurar
 * detalhes do modelo de dados, como chaves primárias, chaves estrangeiras, índices,
 * relacionamentos entre tabelas, e outras restrições, usando a Fluent API do EF Core.
 *
 * -------------------------------------------------------------------------------------------------
 * !!! ATENÇÃO: COMANDOS DO EF CORE PARA MODIFICAÇÕES NO BANCO DE DADOS !!!
 * -------------------------------------------------------------------------------------------------
 * Sempre que fizer alterações nesta classe (TodoDbContext) ou nas classes de
 * modelo (Models como TaskModel, UserModel) que afetem a estrutura do banco de dados
 * (ex: adicionar/remover propriedades, alterar tipos de dados, adicionar índices,
 * modificar relacionamentos), será necessário executar os seguintes comandos do EF Core
 * no terminal, na pasta raiz do seu projeto (onde está o arquivo .csproj):
 *
 * 1. PARA CRIAR UMA NOVA MIGRAÇÃO (substitua 'NomeDescritivoDaSuaMigration' por um nome
 * que descreva a alteração que você fez, ex: 'AddEmailToUser' ou 'UpdateTaskIndexes'):
 *
 * dotnet ef migrations add NomeDescritivoDaSuaMigration
 *
 * 2. PARA APLICAR AS MIGRAÇÕES PENDENTES AO BANCO DE DADOS (isso efetivamente
 * cria/altera as tabelas e outros objetos no seu arquivo .db):
 *
 * dotnet ef database update
 *
 * -------------------------------------------------------------------------------------------------
 * Comandos úteis adicionais (usar com cautela, especialmente em produção):
 *
 * - Listar migrations existentes:
 * dotnet ef migrations list
 *
 * - Remover a última migration (APENAS SE ELA AINDA NÃO FOI APLICADA ao banco de dados
 * de produção ou a um banco compartilhado. É mais seguro em desenvolvimento local):
 * dotnet ef migrations remove
 *
 * - Recriar o banco do zero (APENAS EM AMBIENTE DE DESENVOLVIMENTO, pois apaga todos os dados):
 * dotnet ef database drop
 * dotnet ef database update
 * =================================================================================================
 */

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
        // Aqui, estamos dizendo ao EF Core que queremos uma tabela para nossos Usuários
        public DbSet<UserModel> Users { get; set; }


        //Adner: Configurações de quando criarmos o banco de dados.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Chame o base primeiro

            // Configuração para UserModel
            modelBuilder.Entity<UserModel>(entity =>
            {
                entity.HasKey(u => u.Id); // Define a chave primária
                entity.Property(u => u.Username).IsRequired();
                entity.HasIndex(u => u.Username).IsUnique(); //Não pode ter dois username igual
            });

            // Configuração para TaskModel
            modelBuilder.Entity<TaskModel>(entity =>
            {
                entity.HasKey(t => t.Id); // Define a chave primária
                entity.Property(t => t.Title).IsRequired();

                // Configura o relacionamento com UserModel
                entity.HasOne(t => t.User)           // TaskModel tem um User
                      .WithMany()                    // UserModel (implicitamente) tem muitas Tasks,
                      .HasForeignKey(t => t.UserId)  // A chave estrangeira em TaskModel é UserId
                      .IsRequired();                 // Uma tarefa sempre requer um usuário.
            });
        }
    }
}