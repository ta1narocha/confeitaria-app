using ConfeitariaApp.Models;
using ConfeitariaApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ConfeitariaApp.Controllers
{
    public class PedidoController : Controller
    {
        private readonly AppDbContext _context;

        public PedidoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Pedido
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Bolo)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();
            return View(pedidos);
        }

        // GET: Pedido/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Bolos = await _context.Bolos
                .Where(b => b.Status == "Ativo")
                .Include(b => b.Receita)
                .ToListAsync();
            return View();
        }

        // POST: Pedido/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var bolo = await _context.Bolos.FindAsync(pedido.BoloId);
                if (bolo != null)
                {
                    pedido.Valor = bolo.PrecoFinal;
                }

                pedido.DataPedido = DateTime.Now;
                pedido.Status = "Pendente";

                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bolos = await _context.Bolos
                .Where(b => b.Status == "Ativo")
                .Include(b => b.Receita)
                .ToListAsync();
            return View(pedido);
        }

        // GET: Pedido/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Bolo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            ViewBag.Bolos = await _context.Bolos
                .Where(b => b.Status == "Ativo")
                .Include(b => b.Receita)
                .ToListAsync();
            return View(pedido);
        }

        // POST: Pedido/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var bolo = await _context.Bolos.FindAsync(pedido.BoloId);
                    if (bolo != null)
                    {
                        pedido.Valor = bolo.PrecoFinal;
                    }

                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pedidos.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Bolos = await _context.Bolos
                .Where(b => b.Status == "Ativo")
                .Include(b => b.Receita)
                .ToListAsync();
            return View(pedido);
        }

        // GET: Pedido/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Bolo)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }
            return View(pedido);
        }

        // POST: Pedido/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Pedido/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Bolo)
                .ThenInclude(b => b.Receita)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            if (pedido.Bolo == null)
            {
                return NotFound("Bolo não encontrado para este pedido.");
            }

            return View(pedido);
        }

        // POST: Pedido/AtualizarStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatus(int id, string status)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido != null)
            {
                pedido.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Pedido/Relatorio
        [HttpGet]
        public async Task<IActionResult> Relatorio(DateTime? dataInicio, DateTime? dataFim)
        {
            var query = _context.Pedidos.Include(p => p.Bolo).AsQueryable();

            if (dataInicio.HasValue)
            {
                query = query.Where(p => p.DataPedido >= dataInicio.Value);
            }

            if (dataFim.HasValue)
            {
                var fim = dataFim.Value.AddDays(1);
                query = query.Where(p => p.DataPedido < fim);
            }

            var pedidos = await query.OrderByDescending(p => p.DataPedido).ToListAsync();

            ViewBag.DataInicio = dataInicio?.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim?.ToString("yyyy-MM-dd");

            return View(pedidos);
        }
    }
}