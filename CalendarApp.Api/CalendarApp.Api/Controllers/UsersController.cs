using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CalendarApp.Data;
using CalendarApp.DTOs;
using CalendarApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL ����� api/users
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ����������� ������ ������������ - �������� ��� WINDOWS FORM!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        [HttpPost("registeruser")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // ��������� Email
            if (!new EmailAddressAttribute().IsValid(request.Email))
                return BadRequest(new { message = "������� ���������� email" });


            // ��������� ������
            var password = request.Password;
            var regex = new Regex(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@?!_\-=+*/]).{8,}$");
            if (!regex.IsMatch(password))
                return BadRequest(new
                {
                    message = "������ ��� WinForm �� ������������� �����������. " +
                    "������ ������ ���� �� ����� 8 �������� � ���������: " +
                    "�����, �������� � ��������� �����, � ���������� (&#64;?!_-+=*/)"
                });

            // ��������, ��� ������������ � ����� email ��� ����
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return BadRequest(new { message = "������������ � ����� email ��� ����������" });

            // ������� ������ ������������
            var user = new User
            {
                Email = request.Email,
                //PasswordHash = request.Password, // ���� ��� �����������
                Name = request.Name
            };

            // �������� ������
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Email, user.Name });
        }



        // ����������� ������ ������������ - �������� ��� WEB!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
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
                return BadRequest(new
                {
                    message = "������ ��� WEB �� ������������� �����������. " +
                    "������ ������ ���� �� ����� 8 �������� � ���������: " +
                    "�����, �������� � ��������� �����, � ���������� (&#64;?!_-+=*/)"
                });

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

            // �������� ������
            user.PasswordHash = _passwordHasher.HashPassword(user, user.PasswordHash);

            // ���� ������ ������ ��� ���� (����� ������� �����������)
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        //// ���� ������������
        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginRequest login)
        //{
        //    var existingUser = await _context.Users
        //        .FirstOrDefaultAsync(u => u.Email == login.Email && u.PasswordHash == login.PasswordHash);

        //    if (existingUser == null)
        //        return Unauthorized("�������� email ��� ������");

        //    // ��������� ��� ������
        //    //var result = _passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, login.PasswordHash);
        //    //if (result == PasswordVerificationResult.Failed)
        //    //    return Unauthorized("�������� email ��� ������");

        //    return Ok(existingUser);
        //}
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (existingUser == null)
                return Unauthorized(new { message = "�������� email ��� ������" });

            // ��������� �������� ������ ������ ����
            var result = _passwordHasher.VerifyHashedPassword(
                existingUser,
                existingUser.PasswordHash,
                login.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "�������� email ��� ������" });

            return Ok(new { existingUser.Id, existingUser.Email, existingUser.Name });
        }


        //// �������� ���� ������������� (��� �����)
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var users = await _context.Users.ToListAsync();
        //    return Ok(users);
        //}

        // GET: api/Users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.Select(u => new {
                u.Id,
                u.Email,
                u.Name
            }).ToList();
            return Ok(users);
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new { user.Id, user.Email, user.Name });
        }


    }
}
