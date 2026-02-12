using KhosuRoom.Business.Dtos.GroupDtos;
using KhosuRoom.Business.Services.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KhosuRoom.Presentation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetGroups()
    {
       
        var result = await _groupService.GetAllGroupAsnyc();
        return Ok(result);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var result = await _groupService.GetGroupAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody]GroupCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
       var result =  await _groupService.CreateGroupAsync(dto);
        return Ok(result);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateGroup([FromBody]GroupUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
       var result = await _groupService.UpdateGroupAsync(dto);
        return Ok(result);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(Guid id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
       var result = await _groupService.DeleteGroupAsync(id);
        return Ok(result);
    }

}
