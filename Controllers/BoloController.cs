using ConfeitariaApp.Data;
using ConfeitariaApp.Models;
using ConfeitariaApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConfeitariaApp.Controllers
{
    public class BoloController(AppDbContext context, CalculadoraPrecoService calculadoraService) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly CalculadoraPrecoService _calculadoraService = calculadoraService;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bolos = await _context.Bolos
                .Include(b => b.Receita)
                .ToListAsync();
            return View(bolos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Receitas = await _context.Receitas.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bolo bolo)
        {
            ModelState.Clear(); 

            if (ModelState.IsValid)
            {
                var receita = await _context.Receitas
                    .Include(r => r.Itens).ThenInclude(i => i.Ingrediente)
                    .FirstOrDefaultAsync(r => r.Id == bolo.ReceitaId);

                if (receita != null)
                {
                    var precoCalculado = await _calculadoraService.CalcularPrecoBolo(bolo, receita);
                    bolo.CustoIngredientes = precoCalculado.CustoIngredientes;
                    bolo.PrecoFinal = precoCalculado.PrecoFinal;
                    bolo.DetalhamentoPreco = System.Text.Json.JsonSerializer.Serialize(precoCalculado);

                    _context.Add(bolo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Receitas = await _context.Receitas.ToListAsync();
            return View(bolo);
        }


        // GET: Bolo/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var bolo = await _context.Bolos.FindAsync(id);
            if (bolo == null) return NotFound();

            ViewBag.Receitas = await _context.Receitas.ToListAsync();
            return View(bolo);
        }

        // POST: Bolo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Bolo bolo)
        {
            if (id != bolo.Id) return NotFound();

            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var receita = await _context.Receitas
                    .Include(r => r.Itens).ThenInclude(i => i.Ingrediente)
                    .FirstOrDefaultAsync(r => r.Id == bolo.ReceitaId);

                if (receita != null)
                {
                    var precoCalculado = await _calculadoraService.CalcularPrecoBolo(bolo, receita);
                    bolo.CustoIngredientes = precoCalculado.CustoIngredientes;
                    bolo.PrecoFinal = precoCalculado.PrecoFinal;

                    _context.Update(bolo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Receitas = await _context.Receitas.ToListAsync();
            return View(bolo);
        }

        // GET: Bolo/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var bolo = await _context.Bolos
                .Include(b => b.Receita)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (bolo == null) return NotFound();

            return View(bolo);
        }

        // POST: Bolo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bolo = await _context.Bolos.FindAsync(id);
            if (bolo != null)
            {
                _context.Bolos.Remove(bolo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
