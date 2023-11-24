namespace Garage_Management.DTO;
using System.ComponentModel.DataAnnotations;

public class CustomerDTO
    {
    public int CustomerId { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    public string Phone { get; set; }

    public string Address { get; set; }

}

