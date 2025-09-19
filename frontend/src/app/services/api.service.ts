import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment'; // <- путь от services
import { Note } from '../models/note.model';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  //private baseUrl = 'https://localhost:7105/api';
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  //  // Регистрация
  // register(user: any): Observable<any> {
  //   return this.http.post(`${this.baseUrl}/Users/register`, user);
  // }

  // // Авторизация
  // login(user: any): Observable<any> {
  //   return this.http.post(`${this.baseUrl}/Users/login`, user);
  // }

  // // Получение заметок по ID пользователя
  // getNotes(userId: number): Observable<any> {
  //   return this.http.get(`${this.baseUrl}/Notes/user/${userId}`);
  // }

  // // Добавление новой заметки
  // addNote(note: any): Observable<any> {
  //   return this.http.post(`${this.baseUrl}/Notes`, note);
  // }

  // // Обновление заметки
  // updateNote(id: number, note: any): Observable<any> {
  //   return this.http.put(`${this.baseUrl}/Notes/${id}`, note);
  // }
  
  // // Удаление заметки
  // deleteNote(id: number): Observable<any> {
  //   return this.http.delete(`${this.baseUrl}/Notes/${id}`);
  // }

  
  
  // Регистрация
  register(user: User): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/Users/register`, user);
  }

  // Авторизация
  login(payload: { email: string; password: string }): Observable<User> {
    return this.http.post<User>(`${this.baseUrl}/Users/login`, payload);
  }

  // Получение заметок по ID пользователя
  getNotes(userId: number): Observable<Note[]> {
    return this.http.get<Note[]>(`${this.baseUrl}/Notes/user/${userId}`);
  }

  // Добавление новой заметки
  addNote(note: Note): Observable<Note> {
    return this.http.post<Note>(`${this.baseUrl}/Notes`, note);
  }

  // Обновление заметки
  updateNote(id: number, note: Note): Observable<Note> {
    return this.http.put<Note>(`${this.baseUrl}/Notes/${id}`, note);
  }

  // Удаление заметки — backend у тебя возвращает список заметок -> Observable<Note[]>
  deleteNote(id: number): Observable<Note[]> {
    return this.http.delete<Note[]>(`${this.baseUrl}/Notes/${id}`);
  }
}
