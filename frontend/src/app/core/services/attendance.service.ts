import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Attendance } from '../models/attendance.model';

@Injectable({ providedIn: 'root' })
export class AttendanceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/attendance`;

  getAll(date?: string, phaseId?: string): Observable<Attendance[]> {
    let params = new HttpParams();
    if (date) params = params.set('date', date);
    if (phaseId) params = params.set('phaseId', phaseId);
    return this.http.get<Attendance[]>(this.baseUrl, { params });
  }

  getToday(phaseId?: string): Observable<Attendance[]> {
    let params = new HttpParams();
    if (phaseId) params = params.set('phaseId', phaseId);
    return this.http.get<Attendance[]>(`${this.baseUrl}/today`, { params });
  }

  create(request: { childId: string; date: string; status: string; notes?: string }): Observable<string> {
    return this.http.post<string>(this.baseUrl, request);
  }

  createBatch(request: { date: string; records: { childId: string; status: string; notes?: string }[] }): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/batch`, request);
  }

  update(id: string, request: { status: string; notes?: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { id, ...request });
  }
}
