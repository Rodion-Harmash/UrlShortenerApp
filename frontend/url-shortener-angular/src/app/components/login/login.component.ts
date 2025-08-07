import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginDto } from '../../models/login.dto';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginData: LoginDto = {
    userName: '',
    password: ''
  };

  error: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login(): void {
    this.authService.login(this.loginData).subscribe({
      next: () => {
        this.error = null;
        this.router.navigate(['/urls']);
      },
      error: (err) => {
        this.error = 'Login failed. Please try again.';
        console.error(err);
      }
    });
  }
}
