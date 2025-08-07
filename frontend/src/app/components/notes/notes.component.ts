import { Component } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-notes',
  standalone: true,
  templateUrl: './notes.component.html',
  // styleUrls: ['./notes.component.css'],
  styleUrl: './notes.component.css',
  imports: [FormsModule, CommonModule]
})
export class NotesComponent {
  notes: any[] = [];
  user: any;
  
  title = '';
  description = '';
  date = '';

  errorMessage = '';

  editNoteData: any = null;
  deleteNoteData: any = null;

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit() {
    const userData = localStorage.getItem('user');
    if (userData) {
      this.user = JSON.parse(userData);
      this.loadNotes();
    }
  }

  loadNotes() {
    this.api.getNotes(this.user.id).subscribe({
      next: (data) => this.notes = data,
      error: () => {
        this.errorMessage = 'Ошибка загрузки заметок.';
      }
    });
  }

  addNote() {
    this.api.addNote({
      title: this.title,
      description: this.description,
      date: this.date,
      color: 'blue',
      imageUrl: '',
      userId: this.user.id
    }).subscribe({
      next: (note) => {
        this.notes.push(note);
        this.title = '';
        this.description = '';
        this.date = '';
      },
      error: (err) => alert('Ошибка добавления заметки: ' + err.error)
    });
  }

  logout() {
    localStorage.removeItem('user');
    window.location.href = '/login';
  }

  goHome() {
    localStorage.removeItem('user');
    this.router.navigate(['/']);
  }

  
  openEditModal(note: any) {
    // Копируем данные и форматируем дату
    this.editNoteData = { 
      ...note,
      date: note.date ? note.date.split('T')[0] : '' // превращаем 2025-07-30T00:00:00 в 2025-07-30
    };
  }

  closeEditModal() {
    this.editNoteData = null;
  }

  saveEdit() {
    this.api.updateNote(this.editNoteData.id, this.editNoteData).subscribe({
      next: (updatedNote) => {
        // Обновляем заметку в массиве
        const index = this.notes.findIndex(n => n.id === updatedNote.id);
        if (index !== -1) {
          this.notes[index] = updatedNote;
        }
        this.closeEditModal();
      }
    });
  }

  openDeleteConfirm(note: any) {
    this.deleteNoteData = note;
  }

  cancelDelete() {
    this.deleteNoteData = null;
  }

  confirmDelete() {
    this.api.deleteNote(this.deleteNoteData.id).subscribe({
      next: () => {
        this.notes = this.notes.filter(n => n.id !== this.deleteNoteData.id);
        this.deleteNoteData = null;
      }
    });
  }





}
