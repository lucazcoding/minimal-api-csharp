using MinimalApi.Dominio.Enums;

namespace MinipalApi.Dominio.ModelViews;

public record AdministradorLogado
{

    public string Email { get; set; } = default!; //nao pode ser Nula

    public string Perfil { get; set; } = default!;

    public string Token { get; set; } = default!;
}
