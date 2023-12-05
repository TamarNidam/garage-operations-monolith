namespace Garage_Management.DTO;
public class GaragePermissionDTO
{
    public int PermissionId { get; set; }

    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public int? GarageId { get; set; }
    public string? GarageName { get; set; }
    public bool? CanView { get; set; }

    public bool? CanEdit { get; set; }
}