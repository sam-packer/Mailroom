using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mailroom.Pages;

[Authorize]
[ApiController]
[Route("api/timezone")]
public class TimezoneController : ControllerBase
{
    private readonly MailroomDbContext _context;

    public TimezoneController(MailroomDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTimezone([FromBody] TimezoneDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Timezone)) return BadRequest();

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdStr, out var userId)) return Unauthorized();

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return NotFound();

        if (user.Timezone != dto.Timezone)
        {
            user.Timezone = dto.Timezone;
            await _context.SaveChangesAsync();
        }

        return Ok();
    }

    public class TimezoneDto
    {
        public string Timezone { get; set; } = "";
    }
}