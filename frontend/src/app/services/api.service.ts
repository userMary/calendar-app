import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private baseUrl = 'https://localhost:7105/api';

  constructor(private http: HttpClient) {}

  register(user: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Users/register`, user);
  }

  login(user: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Users/login`, user);
  }

  getNotes(userId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Notes/user/${userId}`);
  }

  addNote(note: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/Notes`, note);
  }

  updateNote(id: number, note: any): Observable<any> {
    return this.http.put(`${this.baseUrl}/Notes/${id}`, note);
  }
  
  deleteNote(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Notes/${id}`);
  }
}
