using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ConfeitariaApp.Models
{
    public class Bolo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public int ReceitaId { get; set; }
        public Receita Receita { get; set; } = null!;
        public decimal FatorMultiplicador { get; set; } = 1.0m;

        [ValidateNever]
        public decimal CustoIngredientes { get; set; }

        [ValidateNever]
        public decimal PrecoFinal { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public string? Status { get; set; } = "Ativo";
        public string? DetalhamentoPreco { get; set; }
        public string? IngredienteJson { get; set; }
    }
}
