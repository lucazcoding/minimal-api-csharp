using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface iAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);

    Administrador? Incluir(Administrador administrador);

    Administrador? BuscaPorId(int id);


    void Apagar(Administrador administrador);

    List<Administrador> Todos (int? pagina);
}
