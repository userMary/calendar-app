import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-welcome',
  standalone: true,
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css'], // всегда во множественном числе и путь свой
  imports: []
})
export class WelcomeComponent {
  constructor(private router: Router) {}

  goTo(page: string) {
    this.router.navigate([`/${page}`]);
  }

}
