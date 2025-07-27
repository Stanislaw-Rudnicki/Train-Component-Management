using System.ComponentModel.DataAnnotations;

namespace TrainComponent.Application.DTOs;

public class ComponentQuantityDto
{
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive integer.")]
    public int Quantity { get; set; }
}
