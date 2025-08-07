import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AboutDto } from '../models/about.dto';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AboutService {
  private apiUrl = `${environment.apiUrl}/about`;

  constructor(private http: HttpClient) {}

  get(): Observable<AboutDto> {
    return this.http.get<AboutDto>(this.apiUrl);
  }

  update(content: string): Observable<any> {
    return this.http.put(this.apiUrl, JSON.stringify(content), {
      headers: new HttpHeaders({ 'Content-Type': 'application/json' })
    });
  }
}
