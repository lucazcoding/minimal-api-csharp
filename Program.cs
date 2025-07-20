var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Ola pessoal");


app.MapPost("/login", (MnimalApi.DTOs.LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();


});

app.MapPost("/teste", (MnimalApi.DTOs.Usuario usuario1) =>
{
    if (usuario1.Email == "adm@adm.com" && usuario1.Senha == "123456")
        return Results.Ok("Ta logado meu parceiro");
    else
        return Results.Unauthorized();
});

app.Run();






