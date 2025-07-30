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


  //constructor(private api: ApiService) {}
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
      //error: (err) => alert('Ошибка загрузки заметок: ' + err.error)
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
      // error: () => {
      //   this.errorMessage = 'Ошибка добавления заметки.';
      // }
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

  editNote(note: any) {
    const newTitle = prompt('Новый заголовок:', note.title);
    const newDesc = prompt('Новое описание:', note.description);
    const newDate = prompt('Новая дата (в формате ГГГГ-ММ-ДД):', note.date.split('T')[0]);
  
    if (newTitle !== null && newDesc !== null && newDate !== null) {
      this.api.updateNote(note.id, {
        ...note,
        title: newTitle,
        description: newDesc,
        date: newDate
      }).subscribe({
        next: (updatedNote) => {
          note.title = updatedNote.title;
          note.description = updatedNote.description;
          note.date = updatedNote.date;
        }
      });
    }
  }


  // Теперь и этот код РАБОТАЕТ, после редактирования кода на сервере!!!!
  // На сервере в метод Delete добавили возвращение обновленного массива заметок после удаления
  // Удаляет с экрана заметки!!!!!!!!!!
  deleteNote(id: number) {
    if (confirm('Удалить заметку?')) {
      this.api.deleteNote(id).subscribe({
        next: () => {
          this.notes = this.notes.filter(n => n.id !== id); // удаляем из массива
        },
        error: () => {
          this.errorMessage = 'Ошибка при удалении.';
        }
        // error: (err) => {
        //   console.log(err);
        //   alert('Ошибка при удалении');
        // }
      });
    }
  }
  


  // РАБОТАЕТ код ниже удаляет с экрана заметки!!!!!!!!!!
  // deleteNote(id: number) {
  //   if (confirm('Удалить заметку?')) {
  //     this.api.deleteNote(id).subscribe({
  //       next: (updatedNotes: any) => {
  //         // Обновляем массив заметок на то, что пришло с сервера
  //         this.notes = updatedNotes;
  //       },
  //       error: (err) => {
  //         console.log(err);
  //         alert('Ошибка при удалении');
  //       }
  //     });
  //   }
  // }



  // РАБОТАЕТ код ниже удаляет с экрана заметки!!!!!!!!!
  // deleteNote(id: number) {
  //   if (confirm('Удалить заметку?')) {
  //     this.api.deleteNote(id).subscribe({
  //       next: () => {
  //         // Загружаем актуальные данные с сервера
  //         this.loadNotes();
  //       },
  //       error: (err) => {
  //         console.log(err);
  //         alert('Ошибка при удалении');
  //       }
  //     });
  //   }
  // }


  editNoteData: any = null;
  deleteNoteData: any = null;

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



// import { Component } from '@angular/core';
// import { ApiService } from '../../services/api.service';
// import { Router } from '@angular/router';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-notes',
//   standalone: true,
//   templateUrl: './notes.component.html',
//   styleUrls: ['./notes.component.css'],
//   imports: [FormsModule, CommonModule]
// })
// export class NotesComponent {
//   notes: any[] = [];
//   user: any = null;

//   editNoteData: any = null;
//   deleteNoteData: any = null;

//   constructor(private api: ApiService, private router: Router) {}

//   ngOnInit() {
//     const userData = localStorage.getItem('user');
//     if (userData) {
//       this.user = JSON.parse(userData);
//       this.loadNotes();
//     }
//   }

//   loadNotes() {
//     this.api.getNotes(this.user.id).subscribe({
//       next: (data) => (this.notes = data),
//       error: () => (this.errorMessage = 'Ошибка загрузки заметок')
//     });
//   }

//   goHome() {
//     this.router.navigate(['/']);
//   }

//   logout() {
//     localStorage.removeItem('user');
//     this.router.navigate(['/']);
//   }

//   openEditModal(note: any) {
//     this.editNoteData = { ...note };
//   }

//   closeEditModal() {
//     this.editNoteData = null;
//   }

//   saveEdit() {
//     this.api.updateNote(this.editNoteData.id, this.editNoteData).subscribe({
//       next: (updatedNote) => {
//         // Обновляем массив заметок, заменяя отредактированную
//         this.notes = this.notes.map((n) =>
//           n.id === updatedNote.id ? updatedNote : n
//         );
//         this.closeEditModal();
//       },
//       error: () => alert('Ошибка при редактировании')
//     });
//   }

//   openDeleteConfirm(note: any) {
//     this.deleteNoteData = note;
//   }

//   cancelDelete() {
//     this.deleteNoteData = null;
//   }

//   confirmDelete() {
//     this.api.deleteNote(this.deleteNoteData.id).subscribe({
//       next: () => {
//         this.notes = this.notes.filter(
//           (n) => n.id !== this.deleteNoteData.id
//         );
//         this.deleteNoteData = null;
//       },
//       error: () => alert('Ошибка при удалении')
//     });
//   }
// }
