import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateUserRequest, SystemUser } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserManagementService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/users`;

  getUsers(): Observable<SystemUser[]> {
    return this.http.get<SystemUser[]>(this.baseUrl);
  }

  getRoles(): Observable<string[]> {
    return this.http.get<string[]>(`${this.baseUrl}/roles`);
  }

  createUser(request: CreateUserRequest): Observable<void> {
    return this.http.post<void>(this.baseUrl, request);
  }

  updateUserRoles(id: string, roles: string[]): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/roles`, { roles });
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
