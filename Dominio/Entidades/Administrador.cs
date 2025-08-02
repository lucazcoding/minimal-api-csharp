using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Administrador
{
    [Key] // Para que Id seja minha chave primária
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Para que ele seja auto-incrementado
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Email { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string Senha { get; set; } = default!;
    [Required]
    [StringLength(10)]

    public string Perfil { get; set; } = default!;
}
    
        
    