import { Component, Input, OnInit } from '@angular/core';
import { formatDate } from '@angular/common';
import { ApiService } from '../../services/api.service';

// import { Component, Input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-calendar-view',
//   standalone: true,
//   imports: [FormsModule, CommonModule],
//   templateUrl: './calendar-view.component.html',
//   styleUrls: ['./calendar-view.component.css']
// })

@Component({
  selector: 'app-calendar-view',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './calendar-view.component.html',
  styleUrls: ['./calendar-view.component.css']
})
export class CalendarViewComponent implements OnInit {

  currentYear: number = new Date().getFullYear();
  currentMonth: number = new Date().getMonth();
  weeks: (number | null)[][] = [];
  @Input() notes: any[] = [];
  //notes: any[] = [];
  editNoteData: any = null;
  showConfirmDelete: boolean = false;
  showYearView: boolean = false;
  userId: number = 0;

  constructor(private api: ApiService) {}

  ngOnInit(): void {
    const user = localStorage.getItem('user');
    if (user) {
      this.userId = JSON.parse(user).id;
      this.loadNotes();
    }
    this.generateCalendar();
  }

  // Генерация календаря (массив недель с днями)
  generateCalendar(): void {
    const firstDay = new Date(this.currentYear, this.currentMonth, 1);
    const lastDay = new Date(this.currentYear, this.currentMonth + 1, 0);
    const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1; // с понедельника
    const totalDays = lastDay.getDate();

    const daysArray: (number | null)[] = [];

    for (let i = 0; i < startDay; i++) daysArray.push(null);
    for (let day = 1; day <= totalDays; day++) daysArray.push(day);
    while (daysArray.length % 7 !== 0) daysArray.push(null);

    this.weeks = [];
    for (let i = 0; i < daysArray.length; i += 7) {
      this.weeks.push(daysArray.slice(i, i + 7));
    }
  }

  // Загрузка заметок через API для текущего пользователя
  loadNotes(): void {
    this.api.getNotes(this.userId).subscribe({
      next: (notes) => {
        this.notes = notes;
        this.generateCalendar();
      },
      error: (err) => console.error('Ошибка загрузки заметок', err)
    });
  }

  // Получить заметки для конкретного дня
  getNotesForDay(day: number | null): any[] {
    if (day === null) return [];
    const dateStr = this.formatDate(new Date(this.currentYear, this.currentMonth, day));
    return this.notes.filter(note => note.date.startsWith(dateStr));
  }

  // Формат даты yyyy-MM-dd
  formatDate(date: Date): string {
    return formatDate(date, 'yyyy-MM-dd', 'en-US');
  }

  // Клик по дню — открыть форму создания/редактирования заметки
  onDayClick(day: number | null): void {
    if (day === null) return;
    const dateStr = this.formatDate(new Date(this.currentYear, this.currentMonth, day));
    const existingNote = this.notes.find(n => n.date.startsWith(dateStr));
    this.editNoteData = {
      id: existingNote?.id || null,
      date: dateStr,
      title: existingNote?.title || '',
      description: existingNote?.description || '',
      color: existingNote?.color || 'white',
      imageUrl: existingNote?.imageUrl || ''
    };
  }

  // Клик по заметке внутри дня — редактировать именно эту заметку
  onNoteClick(note: any, event: MouseEvent): void {
    event.stopPropagation();
    this.editNoteData = { ...note };
  }

  closeEditModal(): void {
    this.editNoteData = null;
    this.showConfirmDelete = false;
  }

  // Сохранить новую или отредактированную заметку через API
  saveEdit(): void {
    if (!this.editNoteData.title.trim() || !this.editNoteData.description.trim() || !this.editNoteData.date) {
      alert('Заполните все поля');
      return;
    }

    const notePayload = {
      userId: this.userId,
      title: this.editNoteData.title.trim(),
      description: this.editNoteData.description.trim(),
      date: this.editNoteData.date,
      color: this.editNoteData.color || 'white',
      imageUrl: this.editNoteData.imageUrl || ''
    };

    if (this.editNoteData.id) {
      // Редактирование
      this.api.updateNote(this.editNoteData.id, notePayload).subscribe({
        next: () => {
          this.loadNotes();
          this.closeEditModal();
        },
        error: err => alert('Ошибка обновления заметки: ' + err.message)
      });
    } else {
      // Создание новой
      this.api.addNote(notePayload).subscribe({
        next: () => {
          this.loadNotes();
          this.closeEditModal();
        },
        error: err => alert('Ошибка добавления заметки: ' + err.message)
      });
    }
  }

  // Подтверждение удаления заметки
  confirmDelete(): void {
    this.showConfirmDelete = true;
  }

  // Удаление заметки через API
  deleteNote(): void {
    if (!this.editNoteData?.id) return;
    this.api.deleteNote(this.editNoteData.id).subscribe({
      next: () => {
        this.loadNotes();
        this.closeEditModal();
      },
      error: err => alert('Ошибка удаления заметки: ' + err.message)
    });
  }

  // Переключение на предыдущий месяц
  prevMonth(): void {
    if (this.currentMonth === 0) {
      this.currentMonth = 11;
      this.currentYear--;
    } else {
      this.currentMonth--;
    }
    this.generateCalendar();
  }

  // Переключение на следующий месяц
  nextMonth(): void {
    if (this.currentMonth === 11) {
      this.currentMonth = 0;
      this.currentYear++;
    } else {
      this.currentMonth++;
    }
    this.generateCalendar();
  }

  // Переключение на годовой режим отображения
  toggleYearView(): void {
    this.showYearView = !this.showYearView;
  }

  // Выбор месяца из годового режима
  setMonth(month: number): void {
    this.currentMonth = month;
    this.showYearView = false;
    this.generateCalendar();
  }

  // Генерация мини-календаря для года
  generateMiniCalendar(month: number): (number | string)[] {
    const firstDay = new Date(this.currentYear, month, 1);
    const lastDay = new Date(this.currentYear, month + 1, 0);
    const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;
    const days: (number | string)[] = [];
    for (let i = 0; i < startDay; i++) days.push('');
    for (let d = 1; d <= lastDay.getDate(); d++) days.push(d);
    return days;
  }

  // Получить название месяца по индексу
  getMonthName(monthIndex: number): string {
    const months = [
      'Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
      'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'
    ];
    return months[monthIndex];
  }
}
























// Это работает с горем по полам!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


// import { Component, Input, OnInit } from '@angular/core';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-calendar-view',
//   standalone: true,
//   imports: [FormsModule, CommonModule],
//   templateUrl: './calendar-view.component.html',
//   styleUrls: ['./calendar-view.component.css']
// })
// export class CalendarViewComponent implements OnInit {
//   @Input() notes: any[] = [];

//   currentMonth = new Date().getMonth();
//   currentYear = new Date().getFullYear();

//   weeks: (number | null)[][] = [];

//   showYearView = false;

//   editNoteData: any = null;

//   showConfirmDelete = false;

//   ngOnInit(): void {
//     this.loadNotesFromStorage();
//     this.generateCalendar();
//   }

//   generateCalendar(): void {
//     const firstDay = new Date(this.currentYear, this.currentMonth, 1);
//     const lastDay = new Date(this.currentYear, this.currentMonth + 1, 0);
//     const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1; // Пн=0 ... Вс=6

//     const totalDays = lastDay.getDate();
//     const daysArray: (number | null)[] = [];

//     for (let i = 0; i < startDay; i++) daysArray.push(null);
//     for (let d = 1; d <= totalDays; d++) daysArray.push(d);
//     while (daysArray.length % 7 !== 0) daysArray.push(null);

//     this.weeks = [];
//     for (let i = 0; i < daysArray.length; i += 7) {
//       this.weeks.push(daysArray.slice(i, i + 7));
//     }
//   }

//   getMonthName(monthIndex: number): string {
//     const months = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
//                     'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
//     return months[monthIndex];
//   }

//   formatDateLocal(date: Date): string {
//     return date.toLocaleDateString('sv-SE'); // YYYY-MM-DD
//   }

//   getNotesForDay(day: number | null): any[] {
//     if (!day) return [];
//     const dateStr = this.formatDateLocal(new Date(this.currentYear, this.currentMonth, day));
//     return this.notes.filter(note => this.formatDateLocal(new Date(note.date)) === dateStr);
//   }

//   prevMonth(): void {
//     if (this.showYearView) {
//       this.currentYear--;
//     } else {
//       if (this.currentMonth === 0) {
//         this.currentMonth = 11;
//         this.currentYear--;
//       } else {
//         this.currentMonth--;
//       }
//     }
//     this.generateCalendar();
//   }

//   nextMonth(): void {
//     if (this.showYearView) {
//       this.currentYear++;
//     } else {
//       if (this.currentMonth === 11) {
//         this.currentMonth = 0;
//         this.currentYear++;
//       } else {
//         this.currentMonth++;
//       }
//     }
//     this.generateCalendar();
//   }

//   toggleYearView(): void {
//     this.showYearView = !this.showYearView;
//   }

//   selectMonth(monthIndex: number): void {
//     this.currentMonth = monthIndex;
//     this.showYearView = false;
//     this.generateCalendar();
//   }

//   onDayClick(day: number | null): void {
//     if (day === null) return;

//     const clickedDate = new Date(this.currentYear, this.currentMonth, day);
//     const dateStr = this.formatDateLocal(clickedDate);

//     const existingNote = this.notes.find(n => this.formatDateLocal(new Date(n.date)) === dateStr);

//     this.editNoteData = {
//       id: existingNote?.id || null,
//       date: dateStr,
//       title: existingNote?.title || '',
//       description: existingNote?.description || ''
//     };
//   }

//   onNoteClick(note: any, event: MouseEvent): void {
//     event.stopPropagation();
//     this.editNoteData = {
//       id: note.id,
//       date: this.formatDateLocal(new Date(note.date)),
//       title: note.title,
//       description: note.description
//     };
//   }

//   saveEdit(): void {
//     if (!this.editNoteData.title.trim() || !this.editNoteData.description.trim() || !this.editNoteData.date) {
//       return; // Валидация - все поля обязательны
//     }

//     const note = {
//       id: this.editNoteData.id || Math.random(),
//       date: this.editNoteData.date,
//       title: this.editNoteData.title.trim(),
//       description: this.editNoteData.description.trim()
//     };

//     const index = this.notes.findIndex(n => n.id === note.id);
//     if (index !== -1) {
//       this.notes[index] = note; // обновляем
//     } else {
//       this.notes.push(note); // создаем новую
//     }

//     this.saveNotesToStorage();
//     this.generateCalendar();
//     this.closeEditModal();
//   }

//   closeEditModal(): void {
//     this.editNoteData = null;
//   }

//   confirmDelete(): void {
//     this.showConfirmDelete = true;
//   }

//   deleteNote(): void {
//     if (!this.editNoteData?.id) return;
//     this.notes = this.notes.filter(note => note.id !== this.editNoteData.id);
//     this.saveNotesToStorage();
//     this.showConfirmDelete = false;
//     this.closeEditModal();
//     this.generateCalendar();
//   }

//   saveNotesToStorage(): void {
//     localStorage.setItem('calendar_notes', JSON.stringify(this.notes));
//   }

//   loadNotesFromStorage(): void {
//     const saved = localStorage.getItem('calendar_notes');
//     if (saved) {
//       this.notes = JSON.parse(saved);
//     }
//   }

//   generateMiniCalendar(month: number): (number | null)[] {
//     const year = this.currentYear;
//     const firstDay = new Date(year, month, 1);
//     const lastDay = new Date(year, month + 1, 0);
//     const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;

//     const daysInMonth = lastDay.getDate();
//     const result: (number | null)[] = [];

//     for (let i = 0; i < startDay; i++) result.push(null);
//     for (let d = 1; d <= daysInMonth; d++) result.push(d);
//     while (result.length % 7 !== 0) result.push(null);

//     return result;
//   }
// }












// яяяяяяяяяяяя
// яяяяяяяяяяяя


// яяяяяяяяяяяяя


// яяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяя

// яяяя
// яя
// яяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяяя





// import { Component, Input, OnInit } from '@angular/core';
// import { FormsModule } from '@angular/forms';
// import { CommonModule } from '@angular/common';

// @Component({
//   selector: 'app-calendar-view',
//   standalone: true,
//   imports: [FormsModule, CommonModule],
//   templateUrl: './calendar-view.component.html',
//   styleUrl: './calendar-view.component.css'
// })

// export class CalendarViewComponent implements OnInit {
//   days: (Date | null)[] = [];

//   @Input() notes: any[] = [];

//   currentMonth = new Date().getMonth();
//   currentYear = new Date().getFullYear();

//   weeks: (number | null)[][] = [];

//   showModal = false;
//   selectedDate: string = '';
//   noteTitle: string = '';
//   noteText: string = '';
//   editingNote: any = null;

//   // editNoteData: {
//   //   id: number | null;
//   //   date: string;
//   //   title: string;
//   //   description: string;
//   // } | null = null;


//   //daysInMonth: (number | null)[] = [];

//   ngOnInit(): void {
//     this.generateCalendar();
//     this.loadNotesFromStorage();
//   this.buildCalendar();
//   }

//   // generateCalendar(): void {
//   //   const year = this.currentYear;
//   //   const month = this.currentMonth;

//   //   const firstDayOfMonth = new Date(year, month, 1);
//   //   const lastDayOfMonth = new Date(year, month + 1, 0);
//   //   const daysInMonth = lastDayOfMonth.getDate();

//   //   const skip = (firstDayOfMonth.getDay() + 6) % 7; // ПН = 0, ВТ = 1, ..., ВС = 6

//   //   const allDays: (number | null)[] = [];

//   //   // пустые ячейки до начала месяца
//   //   for (let i = 0; i < skip; i++) {
//   //     allDays.push(null);
//   //   }

//   //   // числа месяца
//   //   for (let d = 1; d <= daysInMonth; d++) {
//   //     allDays.push(d);
//   //   }

//   //   // разбиваем по неделям (по 7 дней)
//   //   this.weeks = [];
//   //   for (let i = 0; i < allDays.length; i += 7) {
//   //     this.weeks.push(allDays.slice(i, i + 7));
//   //   }
//   // }

//   generateCalendar(): void {
//     const firstDay = new Date(this.currentYear, this.currentMonth, 1);
//     const lastDay = new Date(this.currentYear, this.currentMonth + 1, 0);
//     const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;

//     const totalDays = lastDay.getDate();
//     const daysArray: (number | null)[] = [];

//     for (let i = 0; i < startDay; i++) {
//       daysArray.push(null);
//     }

//     for (let day = 1; day <= totalDays; day++) {
//       daysArray.push(day);
//     }

//     while (daysArray.length % 7 !== 0) {
//       daysArray.push(null);
//     }

//     this.weeks = [];
//     for (let i = 0; i < daysArray.length; i += 7) {
//       this.weeks.push(daysArray.slice(i, i + 7));
//     }
//   }

//   // getNotesForDay(day: number | null): any[] {
//   //   if (!day) return [];

//   //   const dateStr = new Date(this.currentYear, this.currentMonth, day)
//   //     .toISOString()
//   //     .split('T')[0];

//   //   return this.notes.filter(n => {
//   //     const noteDate = n.date ? new Date(n.date).toISOString().split('T')[0] : '';
//   //     return noteDate === dateStr;
//   //   });
//   // }

//   // onDayClick(day: number | null): void {
//   //   if (!day) return;

//   //   const dateStr = new Date(this.currentYear, this.currentMonth, day)
//   //     .toISOString()
//   //     .split('T')[0];

//   //   alert(`Добавить заметку на ${dateStr}`); // пока просто alert
//   //   // здесь можно вызвать форму добавления или отправить событие родителю
//   // }

//   getNotesForDay(day: number | null): any[] {
//     if (!day) return [];
//     // const dateStr = new Date(this.currentYear, this.currentMonth, day)
//     //   .toISOString()
//     //   .split('T')[0];

//     const dateStr = this.formatDateLocal(new Date(this.currentYear, this.currentMonth, day));
    
//     //return this.notes.filter(n => new Date(n.date).toISOString().startsWith(dateStr));
//     // return this.notes.filter(n => {
//     //     const noteDate = n.date ? new Date(n.date).toISOString().split('T')[0] : '';
//     //     return noteDate === dateStr;
//     // });
//     return this.notes.filter(note => this.formatDateLocal(new Date(note.date)) === dateStr);
//   }

//   // onDayClick(day: number | null): void {
//   //   // if (!day) return;
//   //   // const selected = new Date(this.currentYear, this.currentMonth, day);
//   //   // this.selectedDate = selected.toISOString().split('T')[0];

//   //   // const existingNote = this.notes.find(n =>
//   //   //   new Date(n.date).toISOString().startsWith(this.selectedDate)
//   //   // );

//   //   // this.editingNote = existingNote || null;
//   //   // this.noteTitle = existingNote?.title || '';
//   //   // this.noteText = existingNote?.text || '';
//   //   // this.showModal = true;

//   //   if (day === null) return;
//   //   const clickedDate = new Date(this.currentYear, this.currentMonth, day);
//   //   this.selectedDate = this.formatDate(clickedDate);

//   //   const existingNote = this.notes.find(note =>
//   //     this.formatDate(new Date(note.date)) === this.selectedDate
//   //   );

//   //   this.editingNote = existingNote || null;
//   //   this.noteTitle = existingNote ? existingNote.title : '';
//   //   this.noteText = existingNote ? existingNote.text : '';
//   //   this.showModal = true;
//   // }

//   // не работает onDayClick(day: number | null): void {
//   //   const dateStr = this.formatDate(day);
//   //   const existingNote = this.notes.find(n =>
//   //     this.formatDate(new Date(n.date)) === dateStr
//   //   );
  
//   //   this.editNoteData = {
//   //     id: existingNote?.id || null,
//   //     date: dateStr,
//   //     title: existingNote?.title || '',
//   //     description: existingNote?.description || ''
//   //   };
//   // }

//   onDayClick(day: number | null): void {
//     if (day === null) return;
  
//     const clickedDate = new Date(this.currentYear, this.currentMonth, day);
//     const dateStr = this.formatDateLocal(clickedDate);
  
//     const existingNote = this.notes.find(n =>
//       this.formatDateLocal(new Date(n.date)) === dateStr
//     );
  
//     this.editNoteData = {
//       id: existingNote?.id || null,
//       date: dateStr,
//       title: existingNote?.title || '',
//       description: existingNote?.description || ''
//     };
  
//     this.showModal = true; // не забудь показать модалку!
//   }

//   closeModal(): void {
//     this.showModal = false;
//     this.noteTitle = '';
//     this.noteText = '';
//     this.editingNote = null;
//   }

//   saveNote(): void {
//     const note = {
//       // id: this.editingNote?.id || Math.random(),
//       // date: this.selectedDate,
//       // title: this.noteTitle,
//       // text: this.noteText
//       id: this.editingNote ? this.editingNote.id : Math.random(),
//       date: this.selectedDate,
//       title: this.noteTitle,
//       text: this.noteText
//     };

//     // if (this.editingNote) {
//     //   const index = this.notes.findIndex(n => n.id === this.editingNote.id);
//     //   if (index !== -1) this.notes[index] = note;
//     // } else {
//     //   this.notes.push(note);
//     // }

//     if (this.editingNote) {
//       const index = this.notes.findIndex(n => n.id === this.editingNote.id);
//       if (index !== -1) this.notes[index] = note;
//     } else {
//       this.notes.push(note);
//     }

//     this.closeModal();
//   }

//   private formatDate(date: Date): string {
//     return date.toISOString().split('T')[0];
//   }

//   formatDateLocal(date: Date): string {
//     return date.toLocaleDateString('sv-SE'); // формат: YYYY-MM-DD
//   }


//   // onNoteClick(note: any, event: MouseEvent): void {
//   //   event.stopPropagation(); // чтобы не срабатывал клик по дню
  
//   //   this.editingNote = note;
//   //   this.selectedDate = this.formatDate(new Date(note.date));
//   //   this.noteTitle = note.title;
//   //   this.noteText = note.text;
//   //   this.showModal = true;
//   // }

//   editNoteData: any = null;




//   onNoteClick(note: any, event: MouseEvent): void {
//     event.stopPropagation();

//     this.editNoteData = {
//       id: note.id,
//       date: this.formatDateLocal(new Date(note.date)),
//       title: note.title,
//       description: note.description
//     };
//   }

//   saveEdit(): void {
//     const note = {
//       id: this.editNoteData.id || Math.random(),
//       date: this.editNoteData.date,
//       title: this.editNoteData.title.trim(),
//       description: this.editNoteData.description.trim()
//     };

//     if (!note.title || !note.description || !note.date) return;

//     const index = this.notes.findIndex(n => n.id === note.id);
//     if (index !== -1) {
//       this.notes[index] = note; // update
//     } else {
//       this.notes.push(note); // create
//     }

//     this.saveNotesToStorage();
//     this.buildCalendar(); // обновляем календарь
//     this.closeEditModal();
//   }

//   closeEditModal(): void {
//     this.editNoteData = null;
//   }

//   // Сохраняем все заметки в localStorage
//   saveNotesToStorage(): void {
//     localStorage.setItem('calendar_notes', JSON.stringify(this.notes));
//   }

//   // Загружаем заметки из localStorage при старте
//   loadNotesFromStorage(): void {
//     const saved = localStorage.getItem('calendar_notes');
//     if (saved) {
//       this.notes = JSON.parse(saved);
//     }
//   }

//   // Перестраиваем календарь (например, после сохранения)
//   buildCalendar(): void {
//     const daysInMonth = new Date(this.currentYear, this.currentMonth + 1, 0).getDate();
//     const startDay = new Date(this.currentYear, this.currentMonth, 1).getDay();
//     const correctedStart = (startDay + 6) % 7;

//     this.days = [];
//     const totalCells = Math.ceil((correctedStart + daysInMonth) / 7) * 7;

//     for (let i = 0; i < totalCells; i++) {
//       const dayNum = i - correctedStart + 1;
//       if (i >= correctedStart && dayNum <= daysInMonth) {
//         this.days.push(new Date(this.currentYear, this.currentMonth, dayNum));
//       } else {
//         this.days.push(null); // пустая клетка
//       }
//     }
//   }




//   deleteNote(): void {
//     if (!this.editNoteData?.id) return;
//     this.notes = this.notes.filter(note => note.id !== this.editNoteData.id);
//     this.saveNotesToStorage();
//     this.editNoteData = null;
//     this.buildCalendar();
//   }




//   getMonthName(monthIndex: number): string {
//     const months = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
//                     'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
//     return months[monthIndex];
//   }
  
//   prevMonth(): void {
//     if (this.currentMonth === 0) {
//       this.currentMonth = 11;
//       this.currentYear--;
//     } else {
//       this.currentMonth--;
//     }
//     this.buildCalendar();
//   }
  
//   nextMonth(): void {
//     if (this.currentMonth === 11) {
//       this.currentMonth = 0;
//       this.currentYear++;
//     } else {
//       this.currentMonth++;
//     }
//     this.buildCalendar();
//   }



//   showYearView: boolean = false;

//   toggleYearView(): void {
//     this.showYearView = !this.showYearView;
//   }

//   selectMonth(month: number): void {
//     this.currentMonth = month;
//     this.showYearView = false;
//     this.buildCalendar();
//   }

//   generateMiniCalendar(month: number): (number | string)[] {
//     const firstDay = new Date(this.currentYear, month, 1);
//     const lastDay = new Date(this.currentYear, month + 1, 0);
//     const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;
//     const days: (number | string)[] = [];
//     for (let i = 0; i < startDay; i++) days.push('');
//     for (let d = 1; d <= lastDay.getDate(); d++) days.push(d);
//     return days;
//   }

//   showConfirmDelete: boolean = false;
  
//     confirmDelete(): void {
//     this.showConfirmDelete = true;
//   }

// }






// // import { Component, Input, OnInit } from '@angular/core';
// // import { CommonModule } from '@angular/common';
// // import { FormsModule } from '@angular/forms';
// // import { ApiService } from '../../services/api.service';


// // @Component({
// //   selector: 'app-calendar-view',
// //   standalone: true,
// //   imports: [CommonModule, FormsModule],
// //   templateUrl: './calendar-view.component.html',
// //   styleUrls: ['./calendar-view.component.css']
// // })
// // export class CalendarViewComponent implements OnInit {

// //   currentDate: Date = new Date();
// //   currentMonth: number = this.currentDate.getMonth();
// //   currentYear: number = this.currentDate.getFullYear();
// //   daysInMonth: Date[] = [];
// //   notes: any[] = [];
// //   selectedNote: any = null;
// //   viewMode: 'month' | 'year' = 'month';
// //   selectedDate: string = '';
// //   userId: number = 0;

// //   newNote = {
// //     title: '',
// //     description: '',
// //     color: 'white',
// //     date: '',
// //     imageUrl: ''
// //   };
// //   title = '';
// //   description = '';
// //   date = '';

// //   colors = [
// //     'white',
// //     '#fffacd', // бледно-желтый
// //     '#d0f0c0', // бледно-зеленый
// //     '#f4cccc', // бледно-красный
// //     '#f3e6ff', // бледно-фиолетовый
// //     '#cfe2f3'  // бледно-синий
// //   ];

// //   constructor(private api: ApiService) {}

// //   ngOnInit() {
// //     const user = localStorage.getItem('user');
// //     if (user) {
// //       this.userId = JSON.parse(user).id;
// //       this.loadNotes();
// //     }
// //     this.generateCalendar();
// //   }

// //   generateCalendar() {
// //     this.daysInMonth = [];
// //     const firstDay = new Date(this.currentYear, this.currentMonth, 1);
// //     const lastDay = new Date(this.currentYear, this.currentMonth + 1, 0);

// //     for (let day = 1; day <= lastDay.getDate(); day++) {
// //       this.daysInMonth.push(new Date(this.currentYear, this.currentMonth, day));
// //     }
// //   }

// //   loadNotes() {
// //     this.api.getNotes(this.userId).subscribe(notes => {
// //       this.notes = notes;
// //     });
// //   }

// //   getNotesForDate(date: Date) {
// //     const formattedDate = formatDate(date, 'yyyy-MM-dd', 'en-US');
// //     return this.notes.filter(note => note.date.startsWith(formattedDate));
// //   }

// //   addNote() {
// //     this.api.addNote({
// //       title: this.title,
// //       description: this.description,
// //       date: this.date,
// //       color: 'blue',
// //       imageUrl: '',
// //       userId: this.user.id
// //     }).subscribe({
// //       next: (note) => {
// //         this.notes.push(note);
// //         this.title = '';
// //         this.description = '';
// //         this.date = '';
// //       },
// //       error: (err) => alert('Ошибка добавления заметки: ' + err.error)
// //     });
// //     this.loadNotes(); // после успешного добавления/редактирования
// //   }

// //   saveEdit() {
// //     if (!this.editNoteData.title || !this.editNoteData.description || !this.editNoteData.date) {
// //       return;
// //     }
  
// //     const user = JSON.parse(localStorage.getItem('user')!);
// //     if (!user || !user.id) {
// //       alert('Пользователь не найден. Выполните вход.');
// //       return;
// //     }
  
// //     const newNote = {
// //       userId: user.id,
// //       title: this.editNoteData.title,
// //       description: this.editNoteData.description,
// //       date: this.editNoteData.date
// //     };
  
// //     // Если это новая заметка
// //     if (!this.editNoteData.id) {
// //       this.api.addNote(newNote).subscribe({
// //         next: () => {
// //           this.loadNotes(); // Загружаем заново все заметки
// //           this.closeEditModal();
// //         },
// //         error: (err) => {
// //           console.error('Ошибка при сохранении заметки', err);
// //         }
// //       });
// //     } else {
// //       // Если редактируем существующую
// //       this.api.updateNote(this.editNoteData.id, newNote).subscribe({
// //         next: () => {
// //           this.loadNotes();
// //           this.closeEditModal();
// //         },
// //         error: (err) => {
// //           console.error('Ошибка при обновлении заметки', err);
// //         }
// //       });
// //     }
// //   }

// //   openModal(date: Date) {
// //     this.selectedDate = formatDate(date, 'yyyy-MM-dd', 'en-US');
// //     this.newNote = {
// //       title: '',
// //       description: '',
// //       color: 'white',
// //       date: this.selectedDate,
// //       imageUrl: ''
// //     };
// //     this.selectedNote = null;
// //     (document.getElementById('noteModal') as HTMLDialogElement)?.showModal();
// //   }

// //   saveNote() {
// //     const noteData = {
// //       ...this.newNote,
// //       userId: this.userId
// //     };

// //     this.api.addNote(noteData).subscribe(() => {
// //       this.loadNotes();
// //       (document.getElementById('noteModal') as HTMLDialogElement)?.close();
// //     });
// //   }

// //   viewNote(note: any) {
// //     this.selectedNote = note;
// //     (document.getElementById('viewModal') as HTMLDialogElement)?.showModal();
// //   }

// //   editNote(note: any) {
// //     this.selectedNote = note;
// //     this.newNote = { ...note };
// //     this.selectedDate = formatDate(new Date(note.date), 'yyyy-MM-dd', 'en-US');
// //     (document.getElementById('noteModal') as HTMLDialogElement)?.showModal();
// //   }

// //   updateNote() {
// //     this.api.updateNote(this.selectedNote.id, this.newNote).subscribe(() => {
// //       this.loadNotes();
// //       (document.getElementById('noteModal') as HTMLDialogElement)?.close();
// //     });
// //   }

// //   deleteNote(noteId: number) {
// //     if (confirm('Удалить эту заметку?')) {
// //       this.api.deleteNote(noteId).subscribe(() => {
// //         this.loadNotes();
// //         (document.getElementById('viewModal') as HTMLDialogElement)?.close();
// //       });
// //     }
// //   }

// //   prev() {
// //     if (this.viewMode === 'month') {
// //       this.currentMonth--;
// //       if (this.currentMonth < 0) {
// //         this.currentMonth = 11;
// //         this.currentYear--;
// //       }
// //     } else {
// //       this.currentYear--;
// //     }
// //     this.generateCalendar();
// //   }

// //   next() {
// //     if (this.viewMode === 'month') {
// //       this.currentMonth++;
// //       if (this.currentMonth > 11) {
// //         this.currentMonth = 0;
// //         this.currentYear++;
// //       }
// //     } else {
// //       this.currentYear++;
// //     }
// //     this.generateCalendar();
// //   }

// //   toggleViewMode() {
// //     this.viewMode = this.viewMode === 'month' ? 'year' : 'month';
// //     this.generateCalendar();
// //   }

// //   setMonth(month: number) {
// //     this.currentMonth = month;
// //     this.viewMode = 'month';
// //     this.generateCalendar();
// //   }

// //   formatDateLabel(date: Date): string {
// //     return formatDate(date, 'd MMMM', 'ru-RU');
// //   }

// //   closeModal(id: string) {
// //     (document.getElementById(id) as HTMLDialogElement)?.close();
// //   }
// // }

// //   @Input() notes: any[] = [];

// //   currentMonth: number = new Date().getMonth();
// //   currentYear: number = new Date().getFullYear();

// //   weeks: (number | null)[][] = [];
// //   days: (Date | null)[] = [];

// //   editNoteData: any = null;

// //   showConfirmDelete: boolean = false;

// //   ngOnInit(): void {
// //     this.loadNotesFromStorage();
// //     this.generateCalendar();
// //   }

// //   generateCalendar(): void {
// //     const firstDay = new Date(this.currentYear, this.currentMonth, 1);
// //     const lastDay = new Date(this.currentYear, this.currentMonth + 1, 0);
// //     const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;

// //     const totalDays = lastDay.getDate();
// //     const daysArray: (number | null)[] = [];

// //     for (let i = 0; i < startDay; i++) {
// //       daysArray.push(null);
// //     }

// //     for (let day = 1; day <= totalDays; day++) {
// //       daysArray.push(day);
// //     }

// //     while (daysArray.length % 7 !== 0) {
// //       daysArray.push(null);
// //     }

// //     this.weeks = [];
// //     for (let i = 0; i < daysArray.length; i += 7) {
// //       this.weeks.push(daysArray.slice(i, i + 7));
// //     }
// //   }

// //   getNotesForDay(day: number | null): any[] {
// //     if (day === null) return [];
// //     const dateStr = this.formatDate(new Date(this.currentYear, this.currentMonth, day));
// //     return this.notes.filter(note => this.formatDate(new Date(note.date)) === dateStr);
// //   }

// //   onDayClick(day: number | null): void {
// //     if (day === null) return;
// //     const clickedDate = new Date(this.currentYear, this.currentMonth, day);
// //     const dateStr = this.formatDate(clickedDate);
// //     const existingNote = this.notes.find(n =>
// //       this.formatDate(new Date(n.date)) === dateStr
// //     );

// //     this.editNoteData = {
// //       id: existingNote?.id || null,
// //       date: dateStr,
// //       title: existingNote?.title || '',
// //       description: existingNote?.description || ''
// //     };
// //   }

// //   onNoteClick(note: any, event: MouseEvent): void {
// //     event.stopPropagation();

// //     this.editNoteData = {
// //       id: note.id,
// //       date: this.formatDate(new Date(note.date)),
// //       title: note.title,
// //       description: note.description
// //     };
// //   }

// //   closeEditModal(): void {
// //     this.editNoteData = null;
// //   }

// //   saveEdit(): void {
// //     const note = {
// //       id: this.editNoteData.id || Math.random(),
// //       date: this.editNoteData.date,
// //       title: this.editNoteData.title.trim(),
// //       description: this.editNoteData.description.trim()
// //     };

// //     if (!note.title || !note.description || !note.date) return;

// //     const index = this.notes.findIndex(n => n.id === note.id);
// //     if (index !== -1) {
// //       this.notes[index] = note;
// //     } else {
// //       this.notes.push(note);
// //     }

// //     this.saveNotesToStorage();
// //     this.generateCalendar();
// //     this.closeEditModal();
// //   }

// //   formatDate(date: Date): string {
// //     const year = date.getFullYear();
// //     const month = String(date.getMonth() + 1).padStart(2, '0');
// //     const day = String(date.getDate()).padStart(2, '0');
// //     return `${year}-${month}-${day}`;
// //   }

// //   saveNotesToStorage(): void {
// //     localStorage.setItem('calendar_notes', JSON.stringify(this.notes));
// //   }

// //   loadNotesFromStorage(): void {
// //     const saved = localStorage.getItem('calendar_notes');
// //     if (saved) {
// //       this.notes = JSON.parse(saved);
// //     }
// //   }

// //   confirmDelete(): void {
// //     this.showConfirmDelete = true;
// //   }

// //   deleteNote(): void {
// //     if (!this.editNoteData?.id) return;

// //     this.notes = this.notes.filter(note => note.id !== this.editNoteData.id);
// //     this.saveNotesToStorage();
// //     this.editNoteData = null;
// //     this.generateCalendar();
// //     this.showConfirmDelete = false;
// //   }

// //   getMonthName(monthIndex: number): string {
// //     const months = ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
// //                     'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'];
// //     return months[monthIndex];
// //   }
  
// //   prevMonth(): void {
// //     if (this.currentMonth === 0) {
// //       this.currentMonth = 11;
// //       this.currentYear--;
// //     } else {
// //       this.currentMonth--;
// //     }
// //     this.generateCalendar();
// //   }
  
// //   nextMonth(): void {
// //     if (this.currentMonth === 11) {
// //       this.currentMonth = 0;
// //       this.currentYear++;
// //     } else {
// //       this.currentMonth++;
// //     }
// //     this.generateCalendar();
// //   }

// //   showYearView: boolean = false;

// // toggleYearView(): void {
// //   this.showYearView = !this.showYearView;
// // }

// // // selectMonth(month: number): void {
// // //   this.currentMonth = month;
// // //   this.showYearView = false;
// // //   this.generateCalendar();
// // // }
// // changeMonth(delta: number): void {
// //   this.currentMonth += delta;
// //   if (this.currentMonth > 11) {
// //     this.currentMonth = 0;
// //     this.currentYear++;
// //   } else if (this.currentMonth < 0) {
// //     this.currentMonth = 11;
// //     this.currentYear--;
// //   }
// //   this.generateCalendar();
// // }
// // setMonth(month: number): void {
// //   this.currentMonth = month;
// //   this.showYearView = false;
// //   this.generateCalendar();
// // }
// // generateMiniCalendar(month: number): (number | string)[] {
// //   const firstDay = new Date(this.currentYear, month, 1);
// //   const lastDay = new Date(this.currentYear, month + 1, 0);
// //   const startDay = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1;
// //   const days: (number | string)[] = [];
// //   for (let i = 0; i < startDay; i++) days.push('');
// //   for (let d = 1; d <= lastDay.getDate(); d++) days.push(d);
// //   return days;
// // }

