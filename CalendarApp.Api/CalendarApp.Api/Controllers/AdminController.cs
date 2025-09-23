using CalendarApp.Data;
using CalendarApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarApp.DTOs;


namespace CalendarApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Admin> _passwordHasher;

        public AdminController(AppDbContext context, IPasswordHasher<Admin> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        //// POST api/Admin/loginadmin
        //[HttpPost("loginadmin")]
        //public async Task<IActionResult> Login([FromBody] LoginRequestAdmin request)
        //{

        //    var admin = await _context.Admins
        //        .FirstOrDefaultAsync(a => a.Email == request.Email && a.PasswordHash == request.Password);

        //    if (admin == null)
        //        return Unauthorized(new { message = "Неверный email или пароль" });

        //    return Ok(new
        //    {
        //        admin.Id,
        //        admin.Email,
        //        admin.Name
        //    });
        //}


        [HttpPost("loginadmin")]
        public async Task<IActionResult> Login([FromBody] LoginRequestAdmin request)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);
            if (admin == null)
                return Unauthorized(new { message = "Неверный логин или пароль ЛОГИН" });

            var result = _passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Неверный логин или пароль ХЭШ" });

            return Ok(new
            {
                admin.Id,
                admin.Email,
                admin.Name
            });
        }

        // Получить всех пользователей
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Email, u.Name })
                .ToListAsync();
            return Ok(users);
        }

        // Удалить пользователя по Id
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Пользователь удален" });
        }
    }
}
