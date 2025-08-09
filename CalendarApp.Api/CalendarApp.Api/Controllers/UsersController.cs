using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
            // �������� Email
            if (!new EmailAddressAttribute().IsValid(user.Email))
                return BadRequest(new { message = "������� ���������� email" });

            // �������� ������ (����� � ������)
            var password = user.PasswordHash;
            var regex = new Regex(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@?!_\-=+*/]).{8,}$");
            if (!regex.IsMatch(password))
                return BadRequest(new { message = "������ �� ������������� �����������" });

            // ���������, ���� �� ��� ������������ � ����� email
            // ��������, ��� ������������ ��� ����������
            //if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            //{
            //    return BadRequest("������������ � ����� email ��� ����������");
            //}
            // ��������, ��� ������������ ��� ����������
            //if (_context.Users.Any(u => u.Email == user.Email))
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))

                return BadRequest(new { message = "������������ � ����� email ��� ����������" });

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
