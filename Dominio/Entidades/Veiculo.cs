using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades;

public class Veiculo
{
    [Key] // Para que Id seja minha chave primária
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Para que ele seja auto-incrementado
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(150)]
    public string Nome{ get; set; } = default!;

    [Required]
    [StringLength(100)]
    public string Marca { get; set; } = default!;


    [Required]

    public int Ano { get; set; } = default!;
}
    
        
    