import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Child, ChildDetail, CreateChildRequest } from '../models/child.model';

@Injectable({ providedIn: 'root' })
export class ChildService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/children`;

  getAll(searchTerm?: string, status?: string, phaseId?: string): Observable<Child[]> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    if (status) params = params.set('status', status);
    if (phaseId) params = params.set('phaseId', phaseId);
    return this.http.get<Child[]>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ChildDetail> {
    return this.http.get<ChildDetail>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateChildRequest): Observable<string> {
    return this.http.post<string>(this.baseUrl, request);
  }

  update(id: string, request: CreateChildRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
