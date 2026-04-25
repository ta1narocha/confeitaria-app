using System.ComponentModel.DataAnnotations;

namespace ConfeitariaApp.Models
{
    public class Configuracao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, 1)]
        public decimal CustoOperacionalPercentual { get; set; } = 0.15m; // 15%

        [Required]
        [Range(0, 5)]
        public decimal MargemLucroPercentual { get; set; } = 0.50m; // 50%

        [Range(0, 1)]
        public decimal TaxaEntrega { get; set; } = 0.07m; // 7%

        public DateTime DataAtualizacao { get; set; } = DateTime.Now;
    }
}