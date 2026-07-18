import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegisterRequest } from '../models/auth.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/auth`;

  private readonly tokenSubject = new BehaviorSubject<string | null>(localStorage.getItem('access_token'));
  readonly token$ = this.tokenSubject.asObservable();

  readonly isLoggedIn = signal(this.tokenSubject.value !== null);
  readonly userEmail = signal<string | null>(localStorage.getItem('user_email'));
  readonly userRoles = signal<string[]>(this.parseRoles(localStorage.getItem('user_roles')));

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, request).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  register(request: RegisterRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/register`, request).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('user_email');
    localStorage.removeItem('user_roles');
    this.tokenSubject.next(null);
    this.isLoggedIn.set(false);
    this.userEmail.set(null);
    this.userRoles.set([]);
    this.router.navigate(['/auth/login']);
  }

  isAuthenticated(): boolean {
    return !!this.tokenSubject.value;
  }

  getToken(): string | null {
    return this.tokenSubject.value;
  }

  getRoles(): string[] {
    return this.userRoles();
  }

  hasRole(role: string): boolean {
    return this.userRoles().includes(role);
  }

  private setSession(response: LoginResponse): void {
    localStorage.setItem('access_token', response.token);
    localStorage.setItem('user_email', response.email);
    localStorage.setItem('user_roles', JSON.stringify(response.roles));
    this.tokenSubject.next(response.token);
    this.isLoggedIn.set(true);
    this.userEmail.set(response.email);
    this.userRoles.set(response.roles);
  }

  private parseRoles(value: string | null): string[] {
    if (!value) return [];
    try { return JSON.parse(value); } catch { return []; }
  }
}
