using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Insfraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;

    public DbContexto(IConfiguration configuracaoAppSettings)
    {

        _configuracaoAppSettings = configuracaoAppSettings;
        
    }



    //Criando tabela para realizar operações no banco de Dados utilizando o Dbset<T>
    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Admin"
                
            }
        );
    }


    //Alteração no metodo Onconfiguring onde eu configuro os Dados do meu Banco
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Verificar se o builder ainda nao foi configurado
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();

            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(stringConexao,
                ServerVersion.AutoDetect(stringConexao)

                );

            }


        }
    }

    
}