using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConfeitariaApp.Models
{
    public class ItemReceita
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ReceitaId { get; set; }

        [ForeignKey("ReceitaId")]
        public virtual Receita? Receita { get; set; }

        [Required]
        public int IngredienteId { get; set; }

        [ForeignKey("IngredienteId")]
        public virtual Ingrediente? Ingrediente { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(0.01, 999999.99)]
        [Display(Name = "Quantidade na Receita")]
        public decimal Quantidade { get; set; }

        [Display(Name = "Unidade")]
        public string UnidadeUsada => Ingrediente?.UnidadeBase ?? "g";

        [StringLength(50)]
        public string Observacao { get; set; } = string.Empty;

  
        [Display(Name = "Custo na Receita")]
        public decimal CustoItem
        {
            get
            {
                if (Ingrediente == null)
                    return 0;

                return Quantidade * Ingrediente.PrecoUnitario;
            }
        }

        [Display(Name = "Exibição")]
        public string Exibicao => Ingrediente != null ?
            $"{Ingrediente.Nome}: {Quantidade}{UnidadeUsada} = R$ {CustoItem:F2}" :
            "Ingrediente não encontrado";
    }
}