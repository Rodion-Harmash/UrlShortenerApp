import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterDto } from '../../models/register.dto';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerData: RegisterDto = {
    userName: '',
    email: '',
    password: '',
    displayName: ''
  };

  errors: string[] = [];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.resetFormData();
  }

  register(): void {
    this.errors = [];

    this.authService.register(this.registerData).subscribe({
      next: () => {
        this.resetFormData();
        this.router.navigate(['/login']);
      },
      error: (err) => {
        if (Array.isArray(err?.error)) {
          this.errors = err.error.map((e: any) => e.description || 'Unknown error');
        } else if (typeof err?.error === 'string') {
          this.errors.push(err.error);
        } else {
          this.errors.push('Registration failed. Please try again.');
        }
        console.error('Registration error:', err);
      }
    });
  }

  private resetFormData(): void {
    this.registerData = {
      userName: '',
      email: '',
      password: '',
      displayName: ''
    };
  }
}
