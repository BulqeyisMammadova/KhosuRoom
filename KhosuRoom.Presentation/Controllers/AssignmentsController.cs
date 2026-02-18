using KhosuRoom.Business.Dtos.AssignmentDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _service;
    public AssignmentsController(IAssignmentService service) => _service = service;

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] AssignmentCreateFormDto dto)
    {
        var result = await _service.CreateAssiggn(dto);
        return Ok(result);
    }
      

    [HttpPut]
    public async Task<IActionResult> Update([FromForm] AssignmentUpdateFormDto dto)
    {
        var result = await _service.UpdateAssiggn(dto);
        return Ok(result);
    }
      

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAssiggn(id);
        return Ok(result);
    }
       
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _service.GetByIdAssiggn(id));

    [HttpGet("group/{groupId:guid}")]
    public async Task<IActionResult> GetAll(Guid groupId)
        => Ok(await _service.GetAllAssiggn(groupId));


  
}
