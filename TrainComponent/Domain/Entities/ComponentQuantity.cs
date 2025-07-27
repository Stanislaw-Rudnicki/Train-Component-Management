namespace TrainComponent.Domain.Entities;

public class ComponentQuantity
{
    public int Id { get; set; }
    public int ComponentId { get; set; }
    public int Quantity { get; set; }
    public Component Component { get; set; } = null!;
}
