import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  // styleUrl: './register.component.css',
  imports: [FormsModule, CommonModule]
})
export class RegisterComponent {
  email = '';
  password = '';
  name = '';
  errorMessage = '';

  constructor(private api: ApiService, private router: Router) {}

  register(form: NgForm) {
    if (form.invalid) return;

    this.api
      .register({
        email: this.email,
        passwordHash: this.password,
        name: this.name
      })
      .subscribe({
        next: (user) => {
          // Сразу сохраняем пользователя в localStorage
          localStorage.setItem('user', JSON.stringify(user));

          //alert('Регистрация успешна!');
          //this.errorMessage = 'Регистрация успешна!';

          // И отправляем на страницу заметок
          this.router.navigate(['/notes']);
        },
        // error: (err: any) => {
        //   this.errorMessage = err.error?.message || 'Ошибка регистрации. Проверьте правильность заполнения полей.';
        // }
        error: (err: any) => {
          const serverMessage = (err.error && err.error.message) ? err.error.message : null;
          this.errorMessage = serverMessage || 'Ошибка регистрации. Проверьте правильность заполнения полей.';
        }
      });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goHome() {
    this.router.navigate(['/']);
  }
}
