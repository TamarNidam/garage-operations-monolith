using Garage_Management.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.DTO
{
    public class GarageVisitDTO
    {
        public int VisitId { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public int? GarageId { get; set; }
        public string? GarageName { get; set; }
        public DateOnly? VisitDate { get; set; }
        public string ServiceDescription { get; set; }
        public decimal? TotalCost { get; set; }
        public bool? CanEdit { get; set; } = false;

    }
}
