using Microsoft.EntityFrameworkCore;
using ConfeitariaApp.Data;
using ConfeitariaApp.Services;
using ConfeitariaApp.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


var connectionString = "Data Source=confeitaria.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<CalculadoraPrecoService>();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseStaticFiles();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>(); 
    context.Database.EnsureCreated();
}

app.Run();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var receitas = context.Receitas.Include(r => r.Itens).ToList();
    foreach (var receita in receitas)
    {
        Console.WriteLine($"Receita: {receita.Nome}");
        Console.WriteLine($"  Ingredientes: {receita.Itens?.Count ?? 0}");
        foreach (var item in receita.Itens ?? new List<ItemReceita>())
        {
            var ingrediente = context.Ingredientes.Find(item.IngredienteId);
            Console.WriteLine($"    - {ingrediente?.Nome}: {item.Quantidade}g");
        }
    }
}