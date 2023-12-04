namespace Garage_Management.DTO
{
    public class CustomerPermissionDTO
    {
        public int PermissionId { get; set; }
        public int? UserId { get; set; }
        public int? CustomerId { get; set; }
        public bool? CanView { get; set; } = false;
        public bool? CanEdit { get; set; } = false;
    }
}
