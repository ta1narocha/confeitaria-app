using ConfeitariaApp.Models;
using ConfeitariaApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace ConfeitariaApp.Controllers
{
    public class ReceitaController : Controller
    {
        private readonly AppDbContext _context;

        public ReceitaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Lista de Receitas 
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var receitas = await _context.Receitas
                .AsNoTracking() 
                .Include(r => r.Itens)
                    .ThenInclude(i => i.Ingrediente)
                .ToListAsync();

            foreach (var receita in receitas)
            {
                var total = receita.CalcularCustoTotal();
                Console.WriteLine($"Receita: {receita.Nome}, Total: {total}");
            }

            return View(receitas);
        }

        // GET: Criar Nova Receita
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Ingredientes = await _context.Ingredientes.ToListAsync();
            return View();
        }

        // POST: Criar Receita com múltiplos ingredientes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Receita receita, List<int> IngredienteIds, List<decimal> Quantidades)
        {

            ModelState.Remove("Itens");

            if (ModelState.IsValid)
            {
                // Salva a receita primeiro para gerar o ID
                _context.Receitas.Add(receita);
                await _context.SaveChangesAsync();

                // Vincula os ingredientes recebidos da View
                if (IngredienteIds != null && IngredienteIds.Count > 0)
                {
                    for (int i = 0; i < IngredienteIds.Count; i++)
                    {
                        if (IngredienteIds[i] > 0 && Quantidades[i] > 0)
                        {
                            var item = new ItemReceita
                            {
                                ReceitaId = receita.Id,
                                IngredienteId = IngredienteIds[i],
                                Quantidade = Quantidades[i]
                            };
                            _context.ItensReceita.Add(item);
                        }
                    }
                    // Salva os itens vinculados
                    await _context.SaveChangesAsync();
                }

                TempData["Mensagem"] = "Receita criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Ingredientes = await _context.Ingredientes.ToListAsync();
            return View(receita);
        }

        // GET: Detalhes da Receita
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Itens)
                .ThenInclude(i => i.Ingrediente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();

            return View(receita);
        }

        // GET: Editar Receita
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Itens)
                .ThenInclude(i => i.Ingrediente)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();

            ViewBag.Ingredientes = await _context.Ingredientes.ToListAsync();
            return View(receita);
        }

        // POST: Editar Receita
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Receita receita, List<int> IngredienteIds, List<decimal> Quantidades, List<int> ItemIds)
        {
            if (id != receita.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualizar dados da receita
                    var receitaExistente = await _context.Receitas.FindAsync(id);
                    if (receitaExistente == null)
                        return NotFound();

                    receitaExistente.Nome = receita.Nome;
                    receitaExistente.Descricao = receita.Descricao;
                    receitaExistente.Rendimento = receita.Rendimento;

                    // Remover itens antigos
                    var itensAntigos = _context.ItensReceita.Where(i => i.ReceitaId == id);
                    _context.ItensReceita.RemoveRange(itensAntigos);

                    // Adicionar novos itens
                    for (int i = 0; i < IngredienteIds.Count; i++)
                    {
                        if (IngredienteIds[i] > 0 && Quantidades[i] > 0)
                        {
                            var novoItem = new ItemReceita
                            {
                                ReceitaId = receita.Id,
                                IngredienteId = IngredienteIds[i],
                                Quantidade = Quantidades[i]
                            };
                            _context.ItensReceita.Add(novoItem);
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["Mensagem"] = "Receita atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Receitas.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Ingredientes = await _context.Ingredientes.ToListAsync();
            return View(receita);
        }

        // GET: Deletar Receita
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var receita = await _context.Receitas
                .Include(r => r.Itens)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receita == null)
                return NotFound();

            return View(receita);
        }

        // POST: Deletar Receita
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita != null)
            {
                var itens = _context.ItensReceita.Where(i => i.ReceitaId == id);
                _context.ItensReceita.RemoveRange(itens);
                _context.Receitas.Remove(receita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: API para buscar ingredientes em JSON
        [HttpGet]

        public async Task<JsonResult> GetIngredientesJson()
        {
            var ingredientes = await _context.Ingredientes
                .Select(i => new {
                    id = i.Id,
                    nome = i.Nome,
                    precoUnitario = i.PrecoUnitario,
                    unidadeBase = i.UnidadeBase
                })
                .ToListAsync();
            return Json(ingredientes);
        }


        // GET: Adicionar Ingrediente
        [HttpGet]
        public async Task<IActionResult> AdicionarIngrediente(int id)
        {
            var receita = await _context.Receitas.FindAsync(id);
            if (receita == null)
            {
                return NotFound();
            }

            ViewBag.ReceitaId = id;
            ViewBag.ReceitaNome = receita.Nome;
            ViewBag.Ingredientes = await _context.Ingredientes.ToListAsync();

            return View();
        }

        // POST: Adicionar Ingrediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarIngrediente(ItemReceita itemReceita)
        {
            if (ModelState.IsValid)
            {
                itemReceita.Id = 0; 
                _context.ItensReceita.Add(itemReceita);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Ingrediente adicionado com sucesso!";
                return RedirectToAction(nameof(Edit), new { id = itemReceita.ReceitaId });
            }

            var receita = await _context.Receitas.FindAsync(itemReceita.ReceitaId);
            ViewBag.ReceitaId = itemReceita.ReceitaId;
            ViewBag.ReceitaNome = receita?.Nome;
            ViewBag.Ingredientes = await _context.Ingredientes.ToListAsync();
            return View(itemReceita);
        }

        // GET: Remover Ingrediente
        [HttpGet]
        public async Task<IActionResult> RemoverIngrediente(int id)
        {
            var item = await _context.ItensReceita
                .Include(i => i.Ingrediente)
                .Include(i => i.Receita)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Remover Ingrediente
        [HttpPost, ActionName("RemoverIngrediente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverIngredienteConfirmed(int id)
        {
            var item = await _context.ItensReceita.FindAsync(id);
            if (item != null)
            {
                var receitaId = item.ReceitaId;
                _context.ItensReceita.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Mensagem"] = "Ingrediente removido com sucesso!";
                return RedirectToAction(nameof(Edit), new { id = receitaId });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}