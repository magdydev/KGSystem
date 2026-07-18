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

  getByChild(childId: string): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(`${this.baseUrl}/child/${childId}`);
  }

  create(request: { childId: string; kgPhaseId: string; academicYearId: string; notes?: string }): Observable<string> {
    return this.http.post<string>(this.baseUrl, request);
  }

  update(id: string, request: { status?: string; notes?: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }
}
