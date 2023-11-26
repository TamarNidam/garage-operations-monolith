
using Garage_Management.Models;
using System.ComponentModel.DataAnnotations;
namespace Garage_Management.DTO;
public class CustomerDTO
    {
    public int CustomerId { get; set; }

    [RegularExpression(@"^[a-zA-Z]+$",
        ErrorMessage = "Invalid characters in the name field.")]
    [Required]
    public required string FirstName { get; set; }

    [RegularExpression(@"^[a-zA-Z]+$",
        ErrorMessage = "Invalid characters in the name field.")]
    [Required]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    //[RegularExpression(@"^(0|\+972\-?|0\s|-)([23489]{1}\d{7}|\d{8})$",
     //   ErrorMessage = "Invalid phone.")]
    [Phone]
    public string Phone { get; set; }

    public string Address { get; set; }

    public static implicit operator CustomerDTO?(Customer? v)
    {
        throw new NotImplementedException();
    }
}

