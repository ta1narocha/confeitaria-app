using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ConfeitariaApp.Models
{
    public class Receita
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome da receita é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        public string Descricao { get; set; } = string.Empty;

        public virtual ICollection<ItemReceita> Itens { get; set; } = new List<ItemReceita>();

        [Display(Name = "Rendimento")]
        public string Rendimento { get; set; } = "1 bolo (2kg)";

        [Display(Name = "Total de Itens")]
        public int QuantidadeItens => Itens?.Count ?? 0;

        public decimal CalcularCustoTotal()
        {
            decimal total = 0;
            foreach (var item in Itens)
            {
                total += item.CustoItem;  
            }
            return total;
        }
        [Display(Name = "Custo Total")]
        public decimal CustoTotal
        {
            get
            {
                if (Itens == null || !Itens.Any()) return 0;

                // Soma direta: Quantidade do item * Preço do Ingrediente
                return Itens.Sum(i => i.Quantidade * (i.Ingrediente?.PrecoUnitario ?? 0));
            }
        }

    }
}