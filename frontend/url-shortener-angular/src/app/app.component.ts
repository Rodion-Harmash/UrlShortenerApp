import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Url Shortener';
  isLoggedIn = false;
  isAdmin = false;
  username: string = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Підписка на статус логіна
    this.authService.isLoggedIn$.subscribe(status => {
      this.isLoggedIn = status;
      this.isAdmin = this.authService.isAdmin();
      this.username = this.extractUsername();
    });

    // Якщо перезавантаження сторінки, оновлюємо ім'я вручну
    if (this.authService.isLoggedIn()) {
      this.username = this.extractUsername();
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/urls']); // <- Переадресація на сторінку з лінками
  }

  extractUsername(): string {
    const token = this.authService.getToken();
    if (!token) return '';

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      // Перевірити поля, які можуть містити ім'я
      return (
        payload['unique_name'] ||
        payload['name'] ||
        payload['sub'] ||
        payload['email'] ||
        'User'
      );
    } catch {
      return 'User';
    }
  }

  getUsername(): string {
    return this.username;
  }
}
