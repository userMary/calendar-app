import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';

  constructor(private api: ApiService, private router: Router) {}

  login() {
    this.api.login({ email: this.email, passwordHash: this.password }).subscribe({
      next: (user) => {
        localStorage.setItem('user', JSON.stringify(user));
        this.router.navigate(['/notes']);
      },
      error: () => {
        this.errorMessage = 'Неверный логин или пароль. Проверьте данные и попробуйте снова.';
      }
    });
  }
  
  goToRegister() {
    this.router.navigate(['/register']);
  }

  goHome() {
    this.router.navigate(['/']);
  }
  
}
