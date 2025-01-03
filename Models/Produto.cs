using System.ComponentModel.DataAnnotations;

namespace FuncionalApiDesenvolvedorIO.Models;

public class Produto
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "O campo {0} é obrigatório!")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório!")]
    public string? Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório!")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo {0} precisa ter o valor maior que 0")]
    public decimal Preco { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório!")]
    public int Estoque { get; set; }
}
