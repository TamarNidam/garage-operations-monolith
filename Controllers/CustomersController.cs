
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage_Management.Models;
using Garage_Management.DTO;
using System.Net;

namespace Garage_Management.Controllers
{
    public class CustomersController : Controller
    {
        private readonly Garage_ManagementContext _context;

        public CustomersController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            try
            {
                var sql = "SELECT * FROM [Customers]";
                var customers = await _context.Customers.FromSqlRaw(sql).ToListAsync();
                var customerDTOs = customers
                        .Select(c => new CustomerDTO
                        {
                            CustomerId = c.CustomerId,
                            FirstName = c.FirstName,
                            LastName = c.LastName,
                            Email = c.Email,
                            Phone = c.Phone,
                            Address = c.Address
                        }).ToList();

                return View(customerDTOs);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var customer = await _context.Customers
                    .FirstOrDefaultAsync(m => m.CustomerId == id);
                if (customer == null)
                {
                    return NotFound();
                }
                var customerDTO = new CustomerDTO
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address
                };
                return View(customerDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,FirstName,LastName,Email,Phone,Address")] CustomerDTO customerDTO)
        {
            try
            { 
            if (ModelState.IsValid)
            {
                    var maxCustomerId = await _context.Customers.MaxAsync(c => (int?)c.CustomerId) ?? 0;
                    var newCustomerId = maxCustomerId + 1;
                    var sql = $"INSERT INTO [Customers] (CustomerId,FirstName,LastName,Email,Phone,Address) VALUES ({newCustomerId}, '{customerDTO.FirstName}, '{customerDTO.LastName}, '{customerDTO.Email}, '{customerDTO.Phone}, '{customerDTO.Address}')";
                    await _context.Database.ExecuteSqlRawAsync(sql);
                    return RedirectToAction(nameof(Index));
            }
            return View(customerDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
            {
                return NotFound();
            }

                var sql = $"SELECT CustomerId,FirstName,LastName,Email,Phone,Address FROM [Customers] WHERE CustomerId = {id}";
                var customer = await _context.Customers.FromSqlRaw(sql).FirstOrDefaultAsync();

                if (customer == null)
            {
                return NotFound();
            }
                var customerDTO = new CustomerDTO
                {
                    CustomerId = customer.CustomerId,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address
                };

                return View(customerDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,FirstName,LastName,Email,Phone,Address")] CustomerDTO customerDTO)
        {
            try
            {
                if (id != customerDTO.CustomerId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Customers] SET FirstName = '{customerDTO.FirstName}',LastName = '{customerDTO.LastName}',Email = '{customerDTO.Email}',Phone = '{customerDTO.Phone}',Address = '{customerDTO.Address}' WHERE CustomerId = {customerDTO.CustomerId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return RedirectToAction(nameof(Index));
                }
                return View(customerDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
