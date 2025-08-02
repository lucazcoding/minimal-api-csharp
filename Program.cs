using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MINIMAL_API.Dominio.Interfaces;
using MINIMAL_API.Dominio.Servicos;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Insfraestrutura.Db;
using MinipalApi.Dominio.ModelViews;

#region Builder
var builder = WebApplication.CreateBuilder(args);


var key = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(key)) key = "123456";


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false

    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<iVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o seu token JWT: "
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
                },


            },
            new string[] { }
        }
    });
});








builder.Services.AddDbContext<DbContexto>(options => options.UseMySql(
    builder.Configuration.GetConnectionString("MySql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))
));



var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion


#region Administradores


string GerarTokentJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);//faz a criptografia desses dados

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil),
    };

    var token = new JwtSecurityToken(
        claims: claims,
        //chamando parametros sem saber a ordem
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials

    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}






app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, iAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);
    if (adm != null)
    {
        string token = GerarTokentJwt(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();


}).AllowAnonymous().WithTags("Administradores");




app.MapPost("/administradores", ([FromBody]AdministradorDTO administradorDTO, iAdministradorServico administradorServico) =>
{
    var validacao = new ErrosdeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("Email não pode ser vazio");
    
    if (string.IsNullOrEmpty(administradorDTO.Senha))
        validacao.Mensagens.Add("Senha não pode ser vazia");
    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("Perfil não pode ser vazio");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()  
    }; 
    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil

    });

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" } )
.WithTags("Administradores");



app.MapGet("/administradores", ([FromQuery] int? pagina, iAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
            
        });
    }
    return Results.Ok(adms);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" } )
.WithTags("Administradores");



app.MapGet("/administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscaPorId(id);    

    if (administrador == null) return Results.NotFound();

    return Results.Ok(new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" } )
.WithTags("Administradores");





app.MapDelete("/administradores/{id}", ([FromRoute] int id, iAdministradorServico administradorServico) =>
{
    var adm = administradorServico.BuscaPorId(id);

    if (adm == null) return Results.NotFound();

    administradorServico.Apagar(adm);

    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" } )
.WithTags("Administradores");




#endregion


#region Veiculos

ErrosdeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosdeValidacao
    { 
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O Nome não pode ser vazio");
    
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A Marca não ficar em branco");
    
    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");


    return validacao;



}



app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) =>
{

    var validacao = validaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" } )
.WithTags("Veiculos");



app.MapGet("/veiculos", ([FromQuery] int? pagina, iVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veiculos");



app.MapGet("/veiculos/{id}", ([FromRoute] int id, iVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" } )
.WithTags("Veiculos");








app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();


    var validacao = validaDTO(veiculoDTO);

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" } )
.WithTags("Veiculos");







app.MapDelete("/veiculos/{id}", ([FromRoute] int id, iVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" } )
.WithTags("Veiculos");


#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion






