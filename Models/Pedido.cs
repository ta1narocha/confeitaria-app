using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ConfeitariaApp.Models
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100)]
        public string Cliente { get; set; } = string.Empty;

        [Required]
        public int BoloId { get; set; }

        [ForeignKey("BoloId")]
        [ValidateNever]
        public virtual Bolo? Bolo { get; set; }

        [Required]
        public DateTime DataEntrega { get; set; }

        [Required]
        [ValidateNever]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        [Required]
        [Range(0.01, 999999.99)]
        [ValidateNever]
        public decimal Valor { get; set; }

        [Required]
        [StringLength(20)]
        [ValidateNever]
        public string Status { get; set; } = "Pendente"; // Pendente, Em produção, Concluído, Cancelado

        [StringLength(500)]
        public string Observacoes { get; set; } = string.Empty;

        [StringLength(50)]
        public string FormaPagamento { get; set; } = string.Empty;
    }
}