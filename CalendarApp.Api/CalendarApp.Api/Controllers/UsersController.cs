using CalendarApp.Data;
using CalendarApp.DTOs;
using CalendarApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL ����� api/users
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ����������� ������ ������������
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // ���������, ���� �� ��� ������������ � ����� email
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("������������ � ����� email ��� ����������");
            }

            // ���� ������ ������ ��� ���� (����� ������� �����������)
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // ���� ������������
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login.Email && u.PasswordHash == login.PasswordHash);

            if (existingUser == null)
                return Unauthorized("�������� email ��� ������");

            return Ok(existingUser);
        }

        // �������� ���� ������������� (��� �����)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
