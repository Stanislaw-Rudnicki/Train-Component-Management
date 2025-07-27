namespace TrainComponent.Domain.Entities;

public class Component
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UniqueNumber { get; set; } = string.Empty;
    public bool CanAssignQuantity { get; set; }
    public ComponentQuantity? Quantity { get; set; }
}
