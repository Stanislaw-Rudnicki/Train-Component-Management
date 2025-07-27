using TrainComponent.Application.DTOs;
using TrainComponent.Domain.Entities;

namespace TrainComponent.Application.Mappers;

public static class ComponentMapper
{
    public static ComponentDto ToDto(this Component component)
    {
        return new ComponentDto
        {
            Id = component.Id,
            Name = component.Name,
            UniqueNumber = component.UniqueNumber,
            CanAssignQuantity = component.CanAssignQuantity,
            Quantity = component.Quantity?.Quantity
        };
    }

    public static Component ToEntity(this ComponentDto dto)
    {
        return new Component
        {
            Name = dto.Name,
            UniqueNumber = dto.UniqueNumber,
            CanAssignQuantity = dto.CanAssignQuantity,
            Quantity =
                dto.CanAssignQuantity && dto.Quantity.HasValue
                    ? new ComponentQuantity { Quantity = dto.Quantity.Value }
                    : null
        };
    }

    public static void UpdateFromDto(this Component component, ComponentDto dto)
    {
        component.Name = dto.Name;
        component.UniqueNumber = dto.UniqueNumber;
        component.CanAssignQuantity = dto.CanAssignQuantity;

        if (dto.CanAssignQuantity)
        {
            component.Quantity ??= new ComponentQuantity();
            component.Quantity.Quantity = dto.Quantity!.Value;
        }
        else
        {
            component.Quantity = null;
        }
    }
}
