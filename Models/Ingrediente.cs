using System.ComponentModel.DataAnnotations;

namespace ConfeitariaApp.Models
{
    public class Ingrediente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Unidade base é obrigatória")]
        [StringLength(10)]
        public string UnidadeBase { get; set; } = string.Empty; // "g", "ml", "un"

        [Required(ErrorMessage = "Tipo de unidade é obrigatório")]
        public string TipoUnidade { get; set; } = "Peso"; // "Peso", "Volume", "Unidade"

        [Required]
        [Range(0.01, 999999.99)]
        public decimal QuantidadeEmbalagem { get; set; } = 1; // Ex: 5 (kg), 1000 (g), 1000 (ml)

        [Required]
        [Range(0.01, 999999.99)]
        public decimal PrecoEmbalagem { get; set; } // Preço total da embalagem

        // Campo calculado: preço por grama ou ml
        [Display(Name = "Preço por Grama/mL")]
        public decimal PrecoUnitario => QuantidadeEmbalagem > 0 ? PrecoEmbalagem / QuantidadeEmbalagem : 0;

        [Display(Name = "Exibição")]
        public string Exibicao => $"{Nome} - {QuantidadeEmbalagem}{UnidadeBase} (R$ {PrecoUnitario:F4}/{UnidadeBase})";
    }
}