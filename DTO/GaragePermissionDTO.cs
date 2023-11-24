namespace Garage_Management.DTO;
public class GaragePermissionDTO
{
    public int PermissionId { get; set; }

    public int? UserId { get; set; }

    public int? GarageId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanEdit { get; set; }

    public GarageDTO Garage { get; set; }

    public UserDTO User { get; set; }
}