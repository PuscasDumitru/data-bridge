using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data;
using Data.Entities;

namespace Teza.Controllers
{
    public class QueryController : Controller
    {
        private readonly RepositoryDbContext _context;

        public QueryController(RepositoryDbContext context)
        {
            _context = context;
        }

        // GET: Query
        public async Task<IActionResult> Index()
        {
            return View(await _context.Query.ToListAsync());
        }

        // GET: Query/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query
                .FirstOrDefaultAsync(m => m.Id == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // GET: Query/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Query/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RawSql,DefaultResponseWithLimit,Description,LastExecuteTime,Count,Size")] Query query)
        {
            if (ModelState.IsValid)
            {
                _context.Add(query);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(query);
        }

        // GET: Query/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query.FindAsync(id);
            if (query == null)
            {
                return NotFound();
            }
            return View(query);
        }

        // POST: Query/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RawSql,DefaultResponseWithLimit,Description,LastExecuteTime,Count,Size")] Query query)
        {
            if (id != query.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(query);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QueryExists(query.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(query);
        }

        // GET: Query/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.Query
                .FirstOrDefaultAsync(m => m.Id == id);
            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }

        // POST: Query/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var query = await _context.Query.FindAsync(id);
            _context.Query.Remove(query);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QueryExists(int id)
        {
            return _context.Query.Any(e => e.Id == id);
        }
    }
}
