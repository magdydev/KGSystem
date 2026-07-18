import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Enrollment } from '../models/enrollment.model';

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/enrollments`;

  getAll(): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(this.baseUrl);
  }

  getByChild(childId: number): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(`${this.baseUrl}/child/${childId}`);
  }

  create(request: { childId: number; kgPhaseId: number; academicYearId: number; notes?: string }): Observable<number> {
    return this.http.post<number>(this.baseUrl, request);
  }

  update(id: number, request: { status?: string; notes?: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }
}
