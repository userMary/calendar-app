import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../services/api.service';
import { Admin } from '../../models/admin.model';

@Component({
  selector: 'app-admin-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './admin-login.component.html',
  styleUrls: ['./admin-login.component.css']
})
export class AdminLoginComponent {
  email: string = '';
  password: string = '';
  errorMessage: string = '';

  constructor(private router: Router, private api: ApiService) {}

  login() {
    this.api.loginAdmin({ email: this.email, password: this.password }).subscribe({
      next: (res: Admin) => {
        // Сохраняем флаг и email админа
        localStorage.setItem('admin', JSON.stringify(res));
        this.errorMessage = ''; // очищаем сообщение об ошибке
        this.router.navigate(['/admin-panel']);
      },
      error: () => {
        this.errorMessage = 'Неверный логин или пароль WEB';
      }
    });
  }

  // login() {
  //   // Заглушка: логин/пароль админа фиксированные
  //   if (this.email === 'admin' && this.password === 'admin123') {
  //     localStorage.setItem('admin', 'true');
  //     this.router.navigate(['/admin-panel']);
  //   } else {
  //     this.errorMessage = 'Неверный логин или пароль';
  //   }
  // }

  goHome() {
    this.router.navigate(['/']);
  }
  goToRegister() {
    this.router.navigate(['/register']);
  }
  goToLogin() {
    this.router.navigate(['/login']);
  }
}
