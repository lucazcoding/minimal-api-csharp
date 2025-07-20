namespace MnimalApi.DTOs;

public class LoginDTO
{
    public string Email { get; set; } = default!; //nao pode ser Nula
    public string Senha { get; set; } = default!;


}


public class Usuario {
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
}

