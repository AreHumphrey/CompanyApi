using CompanyApi.DTOs;
using CompanyApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers;
[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public UserController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpPut("edit/{id}")]
    public async Task<IActionResult> EditUser(string id, UserEditDto userEditDto)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.PhoneNumber = userEditDto.PhoneNumber;
        user.Email = userEditDto.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return Ok();
        }

        return BadRequest(result.Errors);
    }

    [HttpGet("search")]
public IActionResult SearchUsers([FromQuery] UserSearchDto searchDto)
{
    var users = _userManager.Users
        .Where(u => (searchDto.Name == null || u.UserName.Contains(searchDto.Name)) &&
                    (searchDto.Email == null || u.Email.Contains(searchDto.Email)))
        .ToList();

    return Ok(users);
}

}
