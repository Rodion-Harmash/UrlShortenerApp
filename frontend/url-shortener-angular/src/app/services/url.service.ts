import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ShortUrlDto } from '../models/short-url.dto';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private apiUrl = `${environment.apiUrl}/urls`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<ShortUrlDto[]> {
    return this.http.get<ShortUrlDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ShortUrlDto> {
    return this.http.get<ShortUrlDto>(`${this.apiUrl}/${id}`);
  }

  create(originalUrl: string): Observable<any> {
    return this.http.post(this.apiUrl, JSON.stringify(originalUrl), {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    });
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
