using INKED.Application.DTOs.Organizations;
using INKED.Application.Organizations;
using Microsoft.AspNetCore.Mvc;

namespace INKED.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _service;

    public OrganizationController(IOrganizationService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizations = await _service.GetAllAsync();
        return Ok(organizations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var organization = await _service.GetByIdAsync(id);

        if (organization == null)
            return NotFound();

        return Ok(organization);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrganizationDto dto)
    {
        try
        {
            var organization = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = organization.OrganizationId }, organization);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateOrganizationDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}