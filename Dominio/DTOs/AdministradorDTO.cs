using MinimalApi.Dominio.Enums;

namespace MinimalApi.DTOs;

public class AdministradorDTO
{
    public string Email { get; set; } = default!; //nao pode ser Nula
    public string Senha { get; set; } = default!;

    public Perfil? Perfil { get; set; } = default!;


}

