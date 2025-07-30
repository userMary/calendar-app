using CalendarApp.Data;
using CalendarApp.DTOs;
using CalendarApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // URL будет api/notes
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotesController(AppDbContext context)
        {
            _context = context;
        }

        // Получить все заметки
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _context.Notes.Include(n => n.User).ToListAsync();
            return Ok(notes);
        }

        // Получить заметки конкретного пользователя по userId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var notes = await _context.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            return Ok(notes);
        }

        // Добавить заметку
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

        // Обновить заметку
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Note note)
        {
            var existingNote = await _context.Notes.FindAsync(id);
            if (existingNote == null) return NotFound("Заметка не найдена");

            existingNote.Title = note.Title;
            existingNote.Description = note.Description;
            existingNote.Date = note.Date;
            existingNote.Color = note.Color;
            existingNote.ImageUrl = note.ImageUrl;

            await _context.SaveChangesAsync();
            return Ok(existingNote);
        }

        // Удалить заметку
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            //var note = await _context.Notes.FindAsync(id);
            //if (note == null) return NotFound("Заметка не найдена");

            //_context.Notes.Remove(note);
            //await _context.SaveChangesAsync();
            //return Ok("Удалено");

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return NotFound(new { message = "Заметка не найдена" });

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            // Возвращаем обновленный список заметок
            var notes = await _context.Notes
                .Where(n => n.UserId == note.UserId)
                .ToListAsync();

            return Ok(notes);
        }
    }
}
