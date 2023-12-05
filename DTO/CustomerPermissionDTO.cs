namespace Garage_Management.DTO
{
    public class CustomerPermissionDTO
    {
        public int PermissionId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public int? CustomerId { get; set; }
        public string? CustomerName { get; set; } 
        public bool? CanView { get; set; } = false;
        public bool? CanEdit { get; set; } = false;
    }
}
