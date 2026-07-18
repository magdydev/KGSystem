import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AcademicYear, CreateAcademicYearRequest, CreateKGPhaseRequest, KGPhase, MonthlyFee, PatchMonthlyFeesRequest } from '../models/reference.model';

@Injectable({ providedIn: 'root' })
export class ReferenceDataService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1`;

  getPhases(): Observable<KGPhase[]> {
    return this.http.get<KGPhase[]>(`${this.baseUrl}/KGPhases`);
  }

  getPhaseById(id: string): Observable<KGPhase> {
    return this.http.get<KGPhase>(`${this.baseUrl}/KGPhases/${id}`);
  }

  createPhase(request: CreateKGPhaseRequest): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/KGPhases`, request);
  }

  updatePhase(id: string, request: CreateKGPhaseRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/KGPhases/${id}`, request);
  }

  deletePhase(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/KGPhases/${id}`);
  }

  getAcademicYears(): Observable<AcademicYear[]> {
    return this.http.get<AcademicYear[]>(`${this.baseUrl}/AcademicYears`);
  }

  getAcademicYearById(id: string): Observable<AcademicYear> {
    return this.http.get<AcademicYear>(`${this.baseUrl}/AcademicYears/${id}`);
  }

  createAcademicYear(request: CreateAcademicYearRequest): Observable<string> {
    return this.http.post<string>(`${this.baseUrl}/AcademicYears`, request);
  }

  updateAcademicYear(id: string, request: CreateAcademicYearRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/AcademicYears/${id}`, request);
  }

  deleteAcademicYear(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/AcademicYears/${id}`);
  }

  getMonthlyFees(): Observable<MonthlyFee[]> {
    return this.http.get<MonthlyFee[]>(`${this.baseUrl}/MonthlyFees`);
  }

  getMonthlyFeeByYearAndMonth(yearId: string, month: number): Observable<MonthlyFee> {
    return this.http.get<MonthlyFee>(`${this.baseUrl}/MonthlyFees/by-year/${yearId}?month=${month}`);
  }

  patchMonthlyFeesByYear(request: PatchMonthlyFeesRequest): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/MonthlyFees/by-year/${request.academicYearId}`, request);
  }
}
