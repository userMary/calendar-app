import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';


@Component({
  selector: 'app-admin-panel',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
  users: any[] = [];
  errorMessage = '';

  constructor(private api: ApiService, private router: Router) {}

  ngOnInit() {
    // if (!localStorage.getItem('admin')) {
    //   this.router.navigate(['/admin-login']);
    //   return;
    // }
    // this.loadUsers();
    const admin = localStorage.getItem('admin');
    if (!admin) {
      this.router.navigate(['/admin-login']);
      return;
    }
    this.loadUsers();

  }

  loadUsers() {
    this.api.getAllUsers().subscribe({
      next: users => this.users = users,
      error: err => this.errorMessage = 'Ошибка загрузки пользователей'
    });
  }

  deleteUser(userId: number) {
    if (!confirm('Вы уверены, что хотите удалить этого пользователя?')) return;
    this.api.deleteUser(userId).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !== userId);
        // Если удалили себя — выходим
        const currentUser = localStorage.getItem('user');
        if (currentUser && JSON.parse(currentUser).id === userId) {
          localStorage.removeItem('user');
        }
      },
      error: err => alert('Ошибка удаления пользователя: ' + err.message)
    });
  }

  logout() {
    localStorage.removeItem('admin');
    this.router.navigate(['/']);
  }
}

