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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Garage_Management.Controllers
{
    public class VehiclesController : Controller
    {
        private const string sql_customer = "SELECT * FROM Customers WHERE CustomerId = {0}";
        private readonly Garage_ManagementContext _context;

        public VehiclesController(Garage_ManagementContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            try
            {
                var sql = "SELECT * FROM Vehicles";
                var Vehicles = await _context.Vehicles.FromSqlRaw(sql).ToListAsync();
                var vehicleDTOs = Vehicles
                    .Select(async v => new VehicleDTO
                    {
                        VehicleId = v.VehicleId,
                        Make = v.Make,
                        Model = v.Model,
                        Year = v.Year,
                        Vin = v.Vin,
                        Mileage = v.Mileage,
                        LastServiceDate = v.LastServiceDate,
                        OwnerId = v.OwnerId,
                        OwnerName = await _context.Customers.FromSqlRaw(sql_customer, v.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                        //Owner = await _context.Customers
                        //    .FromSqlRaw(sql_customer, v.OwnerId)
                        //    .FirstOrDefaultAsync()
                        //Owner = new CustomerDTO
                        //{
                        //    CustomerId = v.Owner.CustomerId,
                        //    FirstName = v.Owner.FirstName,
                        //    LastName = v.Owner.LastName,
                        //    Email = v.Owner.Email,
                        //    Phone = v.Owner.Phone,
                        //    Address = v.Owner.Address
                        //}
                    }).Select(t => t.Result).ToList();

                return View(vehicleDTOs);
            }
            catch 
            {
                return View("Error");
            }
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var vehicle = await _context.Vehicles
                    .Include(v => v.Owner)
                    .FirstOrDefaultAsync(m => m.VehicleId == id);
                if (vehicle == null)
                {
                    return NotFound();
                }

                var vehicleDTO = new VehicleDTO
                {
                    VehicleId = vehicle.VehicleId,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Vin = vehicle.Vin,
                    Mileage = vehicle.Mileage,
                    LastServiceDate = vehicle.LastServiceDate,
                    OwnerId = vehicle.OwnerId,
                    OwnerName = await _context.Customers.FromSqlRaw(sql_customer, vehicle.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                    //Owner = await _context.Customers
                    //        .FromSqlRaw(sql_customer, vehicle.OwnerId)
                    //        .FirstOrDefaultAsync()
                    //Owner = new CustomerDTO
                    //{
                    //    CustomerId = vehicle.Owner.CustomerId,
                    //    FirstName = vehicle.Owner.FirstName,
                    //    LastName = vehicle.Owner.LastName,
                    //    Email = vehicle.Owner.Email,
                    //    Phone = vehicle.Owner.Phone,
                    //    Address = vehicle.Owner.Address
                    //}
                };

                return View(vehicleDTO);
            }
            catch
            {
                return View("Error");
            }
        }
            

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,Make,Model,Year,Vin,Mileage,LastServiceDate,OwnerId,OwnerName")] VehicleDTO vehicleDTO)
        {
            //var sql1 = "INSERT INTO [Vehicles] (VehicleId, Make, Model, Year, Vin, Mileage, LastServiceDate, OwnerId) VALUES (1, 'Toyota', 'Camry', 2022, 'ABC123XYZ789', 50000, '2023-11-26', 1)";
            
            try
            {
                if (ModelState.IsValid)
                {
                    var maxVehicleId = await _context.Vehicles.MaxAsync(v => (int?)v.VehicleId) ?? 0;
                    var newVehicleId = maxVehicleId + 1;
                    var sql = $"INSERT INTO [Vehicles] (VehicleId, Make, Model, Year, Vin, Mileage, LastServiceDate, OwnerId) VALUES ({newVehicleId}, '{vehicleDTO.Make}', '{vehicleDTO.Model}', {vehicleDTO.Year}, '{vehicleDTO.Vin}', {vehicleDTO.Mileage}, '{vehicleDTO.LastServiceDate}' ,{vehicleDTO.OwnerId})";
                    await _context.Database.ExecuteSqlRawAsync(sql);

               //     var vehicle = await _context.Vehicles
               //.Include(v => v.Owner)
               //.FirstOrDefaultAsync(m => m.VehicleId == newVehicleId);

                    //if (vehicle != null)
                    //{
                    //    vehicleDTO.Owner = new CustomerDTO
                    //    {
                    //        CustomerId = vehicle.Owner.CustomerId,
                    //        FirstName = vehicle.Owner.FirstName,
                    //        LastName = vehicle.Owner.LastName,
                    //        Email = vehicle.Owner.Email,
                    //        Phone = vehicle.Owner.Phone,
                    //        Address = vehicle.Owner.Address
                    //    };
                    //}

                    //                var owner = await _context.Customers
                    //.FromSqlRaw(sql_customer, vehicleDTO.OwnerId)
                    //.FirstOrDefaultAsync();

                    //                vehicleDTO.Owner = owner;

                    return RedirectToAction(nameof(Index));
                }

                

                ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", vehicleDTO.OwnerId);
                return View(vehicleDTO);
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var sql = $"SELECT * FROM [Vehicles] WHERE VehicleId = {id}";
                var vehicle = await _context.Vehicles.FromSqlRaw(sql).FirstOrDefaultAsync();
                if (vehicle == null)
                {
                    return NotFound();
                }
                var vehicleDTO = new VehicleDTO
                {
                    VehicleId = vehicle.VehicleId,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Vin = vehicle.Vin,
                    Mileage = vehicle.Mileage,
                    LastServiceDate = vehicle.LastServiceDate,
                    OwnerId = vehicle.OwnerId,
                    OwnerName = await _context.Customers.FromSqlRaw(sql_customer, vehicle.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                    //Owner = new CustomerDTO
                    //{
                    //    CustomerId = vehicle.Owner.CustomerId,
                    //    FirstName = vehicle.Owner.FirstName,
                    //    LastName = vehicle.Owner.LastName,
                    //    Email = vehicle.Owner.Email,
                    //    Phone = vehicle.Owner.Phone,
                    //    Address = vehicle.Owner.Address
                    //}
                };
                ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", vehicle.OwnerId);
                return View(vehicleDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VehicleId,Make,Model,Year,Vin,Mileage,LastServiceDate,OwnerId,OwnerName")] VehicleDTO vehicleDTO)
        {
            try
            {
                if (id != vehicleDTO.VehicleId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var sql = $"UPDATE [Vehicles] SET Make = '{vehicleDTO.Make}', Model = '{vehicleDTO.Model}',Year = {vehicleDTO.Year}, Vin = '{vehicleDTO.Vin}', Mileage = {vehicleDTO.Mileage}, LastServiceDate = '{vehicleDTO.LastServiceDate}', OwnerId = {vehicleDTO.OwnerId} WHERE VehicleId = {vehicleDTO.VehicleId}";
                    await _context.Database.ExecuteSqlRawAsync(sql);

                    return RedirectToAction(nameof(Index));
                }
                ViewData["OwnerId"] = new SelectList(_context.Customers, "CustomerId", "FirstName", vehicleDTO.OwnerId);
                return View(vehicleDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {

                if (id == null)
                {
                    return NotFound();
                }

                var vehicle = await _context.Vehicles
                    .Include(v => v.Owner)
                    .FirstOrDefaultAsync(m => m.VehicleId == id);

                if (vehicle == null)
                {
                    return NotFound();
                }

                var vehicleDTO = new VehicleDTO
                {
                    VehicleId = vehicle.VehicleId,
                    Make = vehicle.Make,
                    Model = vehicle.Model,
                    Year = vehicle.Year,
                    Vin = vehicle.Vin,
                    Mileage = vehicle.Mileage,
                    LastServiceDate = vehicle.LastServiceDate,
                    OwnerId = vehicle.OwnerId,
                    OwnerName = await _context.Customers.FromSqlRaw(sql_customer, vehicle.OwnerId).Select(c => c.FirstName).FirstOrDefaultAsync()
                    //Owner = new CustomerDTO
                    //{
                    //    CustomerId = vehicle.Owner.CustomerId,
                    //    FirstName = vehicle.Owner.FirstName,
                    //    LastName = vehicle.Owner.LastName,
                    //    Email = vehicle.Owner.Email,
                    //    Phone = vehicle.Owner.Phone,
                    //    Address = vehicle.Owner.Address
                    //}
                };

                return View(vehicleDTO);
            }
            catch
            {
                return View("Error");
            }
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int userid, int id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                //var vehicle = await _context.Vehicles
                //    .Include(v => v.Owner)
                //    .FirstOrDefaultAsync(m => m.VehicleId == id);

                if (!VehicleExists(id))
                {
                    return NotFound();
                }

                await _context.Database.ExecuteSqlRawAsync($"DELETE FROM Vehicles WHERE VehicleId = {id}");

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("Error");
            }
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.VehicleId == id);
        }
    }
}
