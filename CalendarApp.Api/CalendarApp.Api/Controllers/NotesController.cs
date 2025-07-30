using CalendarApp.Data;
using CalendarApp.DTOs;
using CalendarApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL ����� api/notes
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        // �������� ��� �������
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _context.Notes.Include(n => n.User).ToListAsync();
            return Ok(notes);
        }

        // �������� ������� ����������� ������������ �� userId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            return Ok(notes);
        }

        // �������� �������
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] NoteRequest noteDto)
        {
            var note = new Note
            {
                Date = noteDto.Date,
                Title = noteDto.Title,
                Description = noteDto.Description,
                Color = noteDto.Color,
                ImageUrl = noteDto.ImageUrl,
                UserId = noteDto.UserId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();
            return Ok(note);
        }

        // �������� �������
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Note note)
        {
            var existingNote = await _context.Notes.FindAsync(id);
            if (existingNote == null) return NotFound("������� �� �������");

            existingNote.Title = note.Title;
            existingNote.Description = note.Description;
            existingNote.Date = note.Date;
            existingNote.Color = note.Color;
            existingNote.ImageUrl = note.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(existingNote);
        }

        // ������� �������
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var note = await _context.Notes.FindAsync(id);
            //if (note == null) return NotFound("������� �� �������");

            //_context.Notes.Remove(note);
            //await _context.SaveChangesAsync();
            //return Ok("�������");

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return NotFound(new { message = "������� �� �������" });

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            // ���������� ����������� ������ �������
            var notes = await _context.Notes
                .Where(n => n.UserId == note.UserId)
                .ToListAsync();

            return Ok(notes);
        }
    }
}
