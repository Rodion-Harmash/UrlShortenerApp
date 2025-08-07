import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { LoginDto } from '../models/login.dto';
import { RegisterDto } from '../models/register.dto';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;

  private isLoggedInSubject = new BehaviorSubject<boolean>(this.hasToken());
  isLoggedIn$ = this.isLoggedInSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(dto: LoginDto): Observable<string> {
    return new Observable(observer => {
      this.http.post<{ token: string }>(`${this.apiUrl}/login`, dto).subscribe({
        next: (res) => {
          const token = res.token;
          localStorage.setItem('token', token);

          const payload = this.decodeTokenPayload(token);
          const role = this.extractRoleFromClaims(payload);
          localStorage.setItem('role', role);

          this.isLoggedInSubject.next(true);
          observer.next(token);
          observer.complete();
        },
        error: (err) => observer.error(err)
      });
    });
  }

  register(dto: RegisterDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, dto);
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    this.isLoggedInSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getRole(): string | null {
    return localStorage.getItem('role');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    return this.getRole() === 'Admin';
  }

  getUserInfo(): { id: string, role: string, displayName: string } | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = this.decodeTokenPayload(token);

      const id = payload.sub;
      const displayName = payload.displayName;
      const role = this.extractRoleFromClaims(payload);

      return { id: String(id), role, displayName };
    } catch {
      return null;
    }
  }

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  private decodeTokenPayload(token: string): any {
    return JSON.parse(atob(token.split('.')[1]));
  }

  private extractRoleFromClaims(payload: any): string {
    const roleClaim =
      payload['role'] ||
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    if (Array.isArray(roleClaim)) {
      return roleClaim.includes('Admin') ? 'Admin' : 'User';
    }

    return roleClaim === 'Admin' ? 'Admin' : 'User';
  }
}
