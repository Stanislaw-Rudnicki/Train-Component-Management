using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainComponent.Application.DTOs;
using TrainComponent.Application.DTOs.Enums;
using TrainComponent.Application.Mappers;
using TrainComponent.Domain.Entities;
using TrainComponent.Infrastructure.ErrorHandling;
using TrainComponent.Infrastructure.Persistence;

namespace TrainComponent.Controllers;

/// <summary>
/// Controller for managing train components.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ComponentsController(TrainDbContext context) : ControllerBase
{
    private readonly TrainDbContext _context = context;

    /// <summary>
    /// Gets a list of components with optional filtering, pagination, and sorting.
    /// </summary>
    /// <param name="configuration">Application configuration (injected)</param>
    /// <param name="query">Search query (applies to Name and UniqueNumber fields)</param>
    /// <param name="canAssignQuantity">Filter by quantity assignment support (true/false)</param>
    /// <param name="page">Page number (starting from 1)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="sortOptions">
    /// Sorting options:
    /// <list type="bullet">
    /// <item><description><c>SortBy</c>: name | uniquenumber</description></item>
    /// <item><description><c>SortDir</c>: asc | desc (defaults to asc if not provided)</description></item>
    /// </list>
    /// If not provided at all, no sorting is applied.
    /// </param>
    /// <returns>List of components matching the filters</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComponentDto>>> GetAll(
        [FromServices] IConfiguration configuration,
        [FromQuery] string? query,
        [FromQuery] bool? canAssignQuantity,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] SortOptionsDto? sortOptions = null
    )
    {
        if (page < 1 || pageSize < 1)
            return BadRequest(
                new ErrorResponse
                {
                    Status = 400,
                    Message = "Page and pageSize must be greater than 0."
                }
            );

        var enableFts = configuration.GetValue<bool>("EnableFullTextSearch");
        var componentsQuery = _context.Components.Include(c => c.Quantity).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            query = query.Trim();

            if (enableFts)
            {
                componentsQuery = _context
                    .Components.FromSqlInterpolated(
                        $@"
                            SELECT * FROM Components
                            WHERE CONTAINS(Name, {query}) OR CONTAINS(UniqueNumber, {query})
                        "
                    )
                    .Include(c => c.Quantity);
            }
            else
            {
                var q = query.ToLower();
                componentsQuery = componentsQuery.Where(c =>
                    c.Name.Contains(q) || c.UniqueNumber.Contains(q)
                );
            }
        }

        if (canAssignQuantity.HasValue)
        {
            componentsQuery = componentsQuery.Where(c =>
                c.CanAssignQuantity == canAssignQuantity.Value
            );
        }

        if (sortOptions?.SortBy is not null)
        {
            var direction = sortOptions.SortDir ?? SortDirection.Asc;

            componentsQuery = (sortOptions.SortBy.Value, direction) switch
            {
                (ComponentSortBy.UniqueNumber, SortDirection.Desc)
                    => componentsQuery.OrderByDescending(c => c.UniqueNumber),
                (ComponentSortBy.UniqueNumber, SortDirection.Asc)
                    => componentsQuery.OrderBy(c => c.UniqueNumber),
                (ComponentSortBy.Name, SortDirection.Desc)
                    => componentsQuery.OrderByDescending(c => c.Name),
                (ComponentSortBy.Name, SortDirection.Asc) => componentsQuery.OrderBy(c => c.Name),
                _ => componentsQuery
            };
        }

        var components = await componentsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => c.ToDto())
            .ToListAsync();

        return Ok(components);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComponentDto>> GetById(int id)
    {
        var c = await _context
            .Components.Include(c => c.Quantity)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (c == null)
            return NotFound(
                new ErrorResponse { Status = 404, Message = $"Component with id {id} not found." }
            );

        return Ok(c.ToDto());
    }

    [HttpPost]
    public async Task<ActionResult> Create(ComponentDto dto)
    {
        var component = dto.ToEntity();

        _context.Components.Add(component);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = component.Id }, component.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, ComponentDto dto)
    {
        var component = await _context
            .Components.Include(c => c.Quantity)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (component == null)
            return NotFound(
                new ErrorResponse { Status = 404, Message = $"Component with id {id} not found." }
            );

        if (dto.CanAssignQuantity && dto.Quantity is null)
        {
            return BadRequest(
                new ErrorResponse
                {
                    Status = 400,
                    Message = "Quantity is required for assignable components."
                }
            );
        }

        component.UpdateFromDto(dto);
        await _context.SaveChangesAsync();
        return Ok(component.ToDto());
    }

    [HttpPost("{id}/assign-quantity")]
    public async Task<ActionResult<ComponentDto>> AssignQuantity(int id, ComponentQuantityDto dto)
    {
        if (dto.Quantity <= 0)
        {
            return BadRequest(
                new ErrorResponse { Status = 400, Message = "Quantity must be a positive integer." }
            );
        }

        var component = await _context
            .Components.Include(c => c.Quantity)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (component == null)
        {
            return NotFound(
                new ErrorResponse { Status = 404, Message = $"Component with id {id} not found." }
            );
        }

        if (!component.CanAssignQuantity)
        {
            return BadRequest(
                new ErrorResponse
                {
                    Status = 400,
                    Message = "This component does not allow quantity assignment."
                }
            );
        }

        component.Quantity ??= new ComponentQuantity();
        component.Quantity.Quantity = dto.Quantity;

        await _context.SaveChangesAsync();

        return Ok(component.ToDto());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var component = await _context.Components.FindAsync(id);
        if (component == null)
            return NotFound(
                new ErrorResponse { Status = 404, Message = $"Component with id {id} not found." }
            );

        _context.Components.Remove(component);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
