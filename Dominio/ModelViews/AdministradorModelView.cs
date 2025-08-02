using MinimalApi.Dominio.Enums;

namespace MinipalApi.Dominio.ModelViews;

public record AdministradorModelView
{

    public int Id { get; set; } = default!;
    public string Email { get; set; } = default!; //nao pode ser Nula

    public string Perfil { get; set; } = default!;
}
