import { DatePipe, LowerCasePipe, UpperCasePipe } from '@angular/common';
import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ChildService } from '../../../core/services/child.service';
import { PaymentService } from '../../../core/services/payment.service';
import { AttendanceService } from '../../../core/services/attendance.service';
import { ToastService } from '../../../core/services/toast.service';
import { ChildDetail } from '../../../core/models/child.model';
import { Payment } from '../../../core/models/payment.model';
import { Attendance } from '../../../core/models/attendance.model';

@Component({
  selector: 'app-child-detail',
  standalone: true,
  imports: [DatePipe, LowerCasePipe, UpperCasePipe, TranslatePipe],
  templateUrl: './child-detail.component.html',
  styleUrl: './child-detail.component.scss',
})
export class ChildDetailComponent implements OnInit {
  private readonly childService = inject(ChildService);
  private readonly paymentService = inject(PaymentService);
  private readonly attendanceService = inject(AttendanceService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly childId = input.required<number>();
  readonly close = output<void>();

  readonly activeTab = signal<'enrollments' | 'payments' | 'attendance'>('enrollments');
  readonly childDetail = signal<ChildDetail | null>(null);
  readonly payments = signal<Payment[]>([]);
  readonly attendance = signal<Attendance[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    const id = this.childId();

    this.childService.getById(id).subscribe({
      next: detail => {
        this.childDetail.set(detail);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toast.error(this.translate.instant('TOAST.LOAD_ERROR'));
      },
    });

    this.paymentService.getByChild(id).subscribe({
      next: list => this.payments.set(list),
      error: () => this.toast.error(this.translate.instant('TOAST.LOAD_ERROR')),
    });

    this.attendanceService.getAll().subscribe({
      next: list => this.attendance.set(list.filter(a => a.childId === id)),
      error: () => this.toast.error(this.translate.instant('TOAST.LOAD_ERROR')),
    });
  }

  setActiveTab(tab: 'enrollments' | 'payments' | 'attendance'): void {
    this.activeTab.set(tab);
  }
}
