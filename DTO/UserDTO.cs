using System.ComponentModel.DataAnnotations;

namespace Garage_Management.DTO;
    public class UserDTO
    {
        public int UserId { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$", 
            ErrorMessage = "Invalid characters in the Username field.")]
        public string Username { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Invalid password. Password must be at least 8 characters long and contain at least one letter and one digit.")]
        public string Password { get; set; }
    }

