using System.ComponentModel.DataAnnotations;

namespace TrainComponent.Application.DTOs;

public class ComponentDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "UniqueNumber is required.")]
    [MaxLength(50, ErrorMessage = "UniqueNumber cannot exceed 50 characters.")]
    public string UniqueNumber { get; set; } = string.Empty;

    public bool CanAssignQuantity { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
    public int? Quantity { get; set; }
}
