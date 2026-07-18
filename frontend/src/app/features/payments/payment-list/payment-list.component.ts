import { AsyncPipe, CurrencyPipe, DatePipe, LowerCasePipe, UpperCasePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Payment } from '../../../core/models/payment.model';
import { PaymentService } from '../../../core/services/payment.service';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { PaymentFormComponent } from '../payment-form/payment-form.component';

@Component({
  selector: 'app-payment-list',
  standalone: true,
  imports: [AsyncPipe, CurrencyPipe, DatePipe, FormsModule, LowerCasePipe, TranslatePipe, UpperCasePipe, ModalComponent, PaymentFormComponent],
  templateUrl: './payment-list.component.html',
  styleUrl: './payment-list.component.scss',
})
export class PaymentListComponent {
  private readonly paymentService = inject(PaymentService);

  readonly months = Array.from({ length: 12 }, (_, i) => i + 1);
  readonly statuses = ['PAID', 'PARTIAL', 'PENDING', 'OVERDUE'];

  readonly monthFilter = signal<number | null>(null);
  readonly yearFilter = signal<number | null>(new Date().getFullYear());
  readonly statusFilter = signal<string | null>(null);

  private readonly filterSubject = new BehaviorSubject<{ month?: number; year?: number; status?: string }>({ year: new Date().getFullYear() });

  readonly payments$: Observable<Payment[]> = this.filterSubject.pipe(
    debounceTime(300),
    distinctUntilChanged(),
    switchMap(f => this.paymentService.getAll(f.month, f.year, f.status)),
  );

  readonly modalOpen = signal(false);
  readonly editPayment = signal<Payment | null>(null);

  onFilterChange(): void {
    this.filterSubject.next({
      month: this.monthFilter() ?? undefined,
      year: this.yearFilter() ?? undefined,
      status: this.statusFilter() ?? undefined,
    });
  }

  openAdd(): void {
    this.editPayment.set(null);
    this.modalOpen.set(true);
  }

  openEdit(payment: Payment): void {
    this.editPayment.set(payment);
    this.modalOpen.set(true);
  }

  onSaved(): void {
    this.modalOpen.set(false);
    this.filterSubject.next({
      month: this.monthFilter() ?? undefined,
      year: this.yearFilter() ?? undefined,
      status: this.statusFilter() ?? undefined,
    });
  }
}
