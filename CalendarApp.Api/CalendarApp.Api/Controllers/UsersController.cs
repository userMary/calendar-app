using CalendarApp.Data;
using CalendarApp.DTOs;
using CalendarApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL будет api/users
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // Регистрация нового пользователя
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Проверяем, есть ли уже пользователь с таким email
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("Пользователь с таким email уже существует");
            }

            // Пока пароль храним как есть (позже сделаем хэширование)
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // Вход пользователя
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login.Email && u.PasswordHash == login.PasswordHash);

            if (existingUser == null)
                return Unauthorized("Неверный email или пароль");

            return Ok(existingUser);
        }

        // Получить всех пользователей (для теста)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
    }
}
