using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;

namespace Garage_Management.Controllers
{
    public class GarageVisitsController : Controller
    {
        private readonly Garage_ManagementContext _context;

        public GarageVisitsController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: GarageVisits
        public async Task<IActionResult> Index()
        {
            var garage_ManagementContext = _context.GarageVisits.Include(g => g.Customer).Include(g => g.Garage);
            return View(await garage_ManagementContext.ToListAsync());
        }

        // GET: GarageVisits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garageVisit = await _context.GarageVisits
                .Include(g => g.Customer)
                .Include(g => g.Garage)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (garageVisit == null)
            {
                return NotFound();
            }

            return View(garageVisit);
        }

        // GET: GarageVisits/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName");
            return View();
        }

        // POST: GarageVisits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisitId,CustomerId,GarageId,VisitDate,ServiceDescription,TotalCost")] GarageVisit garageVisit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(garageVisit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", garageVisit.CustomerId);
            ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garageVisit.GarageId);
            return View(garageVisit);
        }

        // GET: GarageVisits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garageVisit = await _context.GarageVisits.FindAsync(id);
            if (garageVisit == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", garageVisit.CustomerId);
            ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garageVisit.GarageId);
            return View(garageVisit);
        }

        // POST: GarageVisits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisitId,CustomerId,GarageId,VisitDate,ServiceDescription,TotalCost")] GarageVisit garageVisit)
        {
            if (id != garageVisit.VisitId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(garageVisit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarageVisitExists(garageVisit.VisitId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", garageVisit.CustomerId);
            ViewData["GarageId"] = new SelectList(_context.Garages, "GarageId", "GarageName", garageVisit.GarageId);
            return View(garageVisit);
        }

        // GET: GarageVisits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garageVisit = await _context.GarageVisits
                .Include(g => g.Customer)
                .Include(g => g.Garage)
                .FirstOrDefaultAsync(m => m.VisitId == id);
            if (garageVisit == null)
            {
                return NotFound();
            }

            return View(garageVisit);
        }

        // POST: GarageVisits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var garageVisit = await _context.GarageVisits.FindAsync(id);
            if (garageVisit != null)
            {
                _context.GarageVisits.Remove(garageVisit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GarageVisitExists(int id)
        {
            return _context.GarageVisits.Any(e => e.VisitId == id);
        }
    }
}
