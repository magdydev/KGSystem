import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Child, ChildDetail, CreateChildRequest } from '../models/child.model';

@Injectable({ providedIn: 'root' })
export class ChildService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/children`;

  getAll(searchTerm?: string, status?: string, phaseId?: number): Observable<Child[]> {
    let params = new HttpParams();
    if (searchTerm) params = params.set('searchTerm', searchTerm);
    if (status) params = params.set('status', status);
    if (phaseId) params = params.set('phaseId', phaseId);
    return this.http.get<Child[]>(this.baseUrl, { params });
  }

  getById(id: number): Observable<ChildDetail> {
    return this.http.get<ChildDetail>(`${this.baseUrl}/${id}`);
  }

  create(request: CreateChildRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl, request);
  }

  update(id: number, request: CreateChildRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
