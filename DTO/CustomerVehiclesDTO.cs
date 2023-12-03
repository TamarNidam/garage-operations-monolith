namespace Garage_Management.DTO
{ 
public class CustomerVehiclesDTO
{
    public CustomerDTO Customer { get; set; }
    public List<VehicleDTO> Vehicles { get; set; }
}
}