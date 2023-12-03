using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;
using Garage_Management.DTO;

namespace Garage_Management.Controllers
{
    public class GaragesController : Controller
    {
        private readonly Garage_ManagementContext _context;

        public GaragesController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Garages
        public async Task<IActionResult> Index()
        {
            try
            {
                var sql = "SELECT * FROM [Garages]";
                var garages = await _context.Garages.FromSqlRaw(sql).ToListAsync();
                var garageDTOs = garages
                        .Select(g => new GarageDTO
                        {
                            GarageId = g.GarageId,
                            GarageName = g.GarageName,
                            Address = g.Address,
                            PhoneNumber = g.PhoneNumber
                            //GaragePermissions = g.GaragePermissions,
                            //GarageVisits = g.GarageVisits
                        }).ToList();

                return View(garageDTOs); 
                //return View(await _context.Garages.ToListAsync());
            }
            catch (Exception ex)
            {
                return View("Error", ex); ;
            }
        
           
        }

        // GET: Garages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages
                .FirstOrDefaultAsync(m => m.GarageId == id);
            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        // GET: Garages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Garages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GarageId,GarageName,Address,PhoneNumber")] Garage garage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(garage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(garage);
        }

        // GET: Garages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages.FindAsync(id);
            if (garage == null)
            {
                return NotFound();
            }
            return View(garage);
        }

        // POST: Garages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GarageId,GarageName,Address,PhoneNumber")] Garage garage)
        {
            if (id != garage.GarageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(garage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarageExists(garage.GarageId))
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
            return View(garage);
        }

        // GET: Garages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var garage = await _context.Garages
                .FirstOrDefaultAsync(m => m.GarageId == id);
            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        // POST: Garages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var garage = await _context.Garages.FindAsync(id);
            if (garage != null)
            {
                _context.Garages.Remove(garage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GarageExists(int id)
        {
            return _context.Garages.Any(e => e.GarageId == id);
        }
    }
}
