import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Payment } from '../models/payment.model';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/v1/payments`;

  getAll(month?: number, year?: number, status?: string): Observable<Payment[]> {
    let params = new HttpParams();
    if (month) params = params.set('month', month);
    if (year) params = params.set('year', year);
    if (status) params = params.set('status', status);
    return this.http.get<Payment[]>(this.baseUrl, { params });
  }

  getByChild(childId: string): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.baseUrl}/child/${childId}`);
  }

  create(request: { enrollmentId: string; month: number; year: number; amountDue: number; amountPaid: number; discount?: number; dueDate: string; method: string; notes?: string }): Observable<string> {
    return this.http.post<string>(this.baseUrl, request);
  }

  update(id: string, request: { amountPaid?: number; discount?: number; status?: string; method?: string; notes?: string }): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, request);
  }
}
