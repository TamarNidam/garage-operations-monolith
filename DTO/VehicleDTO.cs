namespace Garage_Management.DTO;
using System;
using System.ComponentModel.DataAnnotations;
public class VehicleDTO
{
    public int VehicleId { get; set; }
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Invalid Make")]
    [Required(ErrorMessage = "Make is required")]
    [StringLength(50)]
    public string Make { get; set; }

    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Invalid Make")]
    [Required(ErrorMessage = "Model is required")]
    [StringLength(50)]
    public string Model { get; set; }

    [RegularExpression(@"^\d{4}$", ErrorMessage = "Invalid Year")]
    public int? Year { get; set; }

    [RegularExpression(@"^[A-HJ-NPR-Z\d]{17}$", ErrorMessage = "Invalid VIN")]
    [StringLength(17)]
    public string Vin { get; set; }

    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid Mileage")]
    public int? Mileage { get; set; }

    public DateOnly? LastServiceDate { get; set; }

    public int? OwnerId { get; set; }

    public  CustomerDTO Owner { get; set; }
}

