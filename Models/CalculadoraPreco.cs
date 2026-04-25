namespace ConfeitariaApp.Models  
{
    public static class CalculadoraPreco 
    {
        public static decimal CalcularPreco(decimal custoIngredientes)
        {
            decimal custoOperacional = custoIngredientes * 0.15m;
            decimal precoFinal = (custoIngredientes + custoOperacional) * 1.5m;
            return precoFinal;
        }
    }
} 