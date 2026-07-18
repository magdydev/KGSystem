import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Attendance } from '../models/attendance.model';

@Injectable({ providedIn: 'root' })
export class AttendanceService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/attendance`;

  getAll(date?: string, phaseId?: number): Observable<Attendance[]> {
    let params = new HttpParams();
    if (date) params = params.set('date', date);
    if (phaseId) params = params.set('phaseId', phaseId);
    return this.http.get<Attendance[]>(this.baseUrl, { params });
  }

  getToday(phaseId?: number): Observable<Attendance[]> {
    let params = new HttpParams();
    if (phaseId) params = params.set('phaseId', phaseId);
    return this.http.get<Attendance[]>(`${this.baseUrl}/today`, { params });
  }

  create(request: { childId: number; date: string; status: string; notes?: string }): Observable<number> {
    return this.http.post<number>(this.baseUrl, request);
  }

  createBatch(request: { date: string; records: { childId: number; status: string; notes?: string }[] }): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/batch`, request);
  }

  update(id: number, request: { status: string; notes?: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, { id, ...request });
  }
}
