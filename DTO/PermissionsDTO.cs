namespace Garage_Management.DTO
{
    public class PermissionsDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<CustomerPermissionDTO> PermissionsCustomers { get; set; }
        public List<GaragePermissionDTO> PermissionsGarages { get; set; }
    }
}
