using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Garage_Management.DTO
{
    public class GarageDTO
    {
        public int GarageId { get; set; }

        [Required(ErrorMessage = "Garage name is required")]
        [StringLength(100)]
        public string GarageName { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public ICollection<GaragePermissionDTO> GaragePermissions { get; set; } = new List<GaragePermissionDTO>();

        public ICollection<GarageVisitDTO> GarageVisits { get; set; } = new List<GarageVisitDTO>();
    }
}

