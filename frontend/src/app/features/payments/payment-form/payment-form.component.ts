import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';
import { Observable, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { Payment } from '../../../core/models/payment.model';
import { Child, ChildDetail } from '../../../core/models/child.model';
import { ChildService } from '../../../core/services/child.service';
import { PaymentService } from '../../../core/services/payment.service';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-payment-form',
  standalone: true,
  imports: [AsyncPipe, CurrencyPipe, FormsModule, TranslatePipe, DateInputComponent],
  templateUrl: './payment-form.component.html',
  styleUrl: './payment-form.component.scss',
})
export class PaymentFormComponent {
  private readonly paymentService = inject(PaymentService);
  private readonly childService = inject(ChildService);
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly editPayment = input<Payment | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly saving = signal(false);

  enrollmentId = 0;
  childNameDisplay = '';

  month = new Date().getMonth() + 1;
  year = new Date().getFullYear();
  amountDue = 0;
  amountPaid = 0;
  discount = 0;
  dueDate = '';
  paidDate = new Date().toISOString().substring(0, 10);
  method = 'Cash';
  notes = '';
  receivedBy = '';

  showChildDropdown = false;
  selectedChildId = 0;

  private readonly searchSubject = new Subject<string>();
  readonly searchResults$: Observable<Child[]> = this.searchSubject.pipe(
    debounceTime(300),
    distinctUntilChanged(),
    switchMap(term => term.trim() ? this.childService.getAll(term, 'ACTIVE') : of([])),
  );

  readonly months = Array.from({ length: 12 }, (_, i) => i + 1);
  readonly methods = ['Cash', 'Card', 'Transfer'];

  constructor() {
    const payment = this.editPayment();
    if (payment) {
      this.childNameDisplay = `${payment.childNameAr} / ${payment.childNameEn}`;
      this.month = payment.month;
      this.year = payment.year;
      this.amountDue = payment.amountDue;
      this.amountPaid = payment.amountPaid;
      this.discount = payment.discount ?? 0;
      this.dueDate = payment.dueDate;
      this.paidDate = payment.paidDate ?? '';
      this.method = payment.method;
      this.notes = payment.notes ?? '';
      this.receivedBy = payment.receivedBy ?? '';
    }

    const today = new Date();
    const nextMonth = new Date(today.getFullYear(), today.getMonth() + 1, 0);
    if (!this.dueDate) {
      this.dueDate = nextMonth.toISOString().substring(0, 10);
    }
  }

  onSearchInput(term: string): void {
    this.searchSubject.next(term);
    this.showChildDropdown = true;
  }

  selectChild(child: Child): void {
    this.selectedChildId = child.id;
    this.childNameDisplay = `${child.fullNameAr} / ${child.fullNameEn}`;
    this.showChildDropdown = false;
    this.loadChildEnrollment(child.id);
  }

  private loadChildEnrollment(childId: number): void {
    this.childService.getById(childId).subscribe({
      next: (detail: ChildDetail) => {
        const active = detail.enrollments?.find(e => e.status === 'ACTIVE');
        if (active) {
          this.enrollmentId = active.id;
          this.referenceDataService.getMonthlyFeeByYearAndMonth(active.academicYearId, this.month).subscribe({
            next: fee => { this.amountDue = fee.amount; },
            error: () => {},
          });
        }
      },
    });
  }

  get remaining(): number {
    return this.amountDue - this.amountPaid - this.discount;
  }

  save(): void {
    if (this.saving()) return;
    this.saving.set(true);

    const payload = {
      enrollmentId: this.enrollmentId,
      month: this.month,
      year: this.year,
      amountDue: this.amountDue,
      amountPaid: this.amountPaid,
      discount: this.discount || undefined,
      dueDate: this.dueDate,
      method: this.method,
      notes: this.notes || undefined,
    };

    const done = () => { this.saving.set(false); this.saved.emit(); };
    const fail = () => {
      this.saving.set(false);
      this.toast.error(this.translate.instant('TOAST.SAVE_ERROR'));
    };

    if (this.editPayment()) {
      this.paymentService.update(this.editPayment()!.id, payload).subscribe({ next: done, error: fail });
    } else {
      this.paymentService.create(payload).subscribe({ next: done, error: fail });
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
