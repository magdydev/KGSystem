import { DatePipe, LowerCasePipe } from '@angular/common';
import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ChildService } from '../../../core/services/child.service';
import { PaymentService } from '../../../core/services/payment.service';
import { AttendanceService } from '../../../core/services/attendance.service';
import { ChildDetail } from '../../../core/models/child.model';
import { Payment } from '../../../core/models/payment.model';
import { Attendance } from '../../../core/models/attendance.model';

@Component({
  selector: 'app-child-detail',
  standalone: true,
  imports: [DatePipe, LowerCasePipe, TranslatePipe],
  templateUrl: './child-detail.component.html',
  styleUrl: './child-detail.component.scss',
})
export class ChildDetailComponent implements OnInit {
  private readonly childService = inject(ChildService);
  private readonly paymentService = inject(PaymentService);
  private readonly attendanceService = inject(AttendanceService);

  readonly childId = input.required<string>();
  readonly close = output<void>();

  readonly activeTab = signal<'enrollments' | 'payments' | 'attendance'>('enrollments');
  readonly childDetail = signal<ChildDetail | null>(null);
  readonly payments = signal<Payment[]>([]);
  readonly attendance = signal<Attendance[]>([]);
  readonly loading = signal(true);

  ngOnInit(): void {
    const id = this.childId();

    this.childService.getById(id).subscribe(detail => {
      this.childDetail.set(detail);
      this.loading.set(false);
    });

    this.paymentService.getByChild(id).subscribe(list => this.payments.set(list));

    this.attendanceService.getAll().subscribe(list => {
      this.attendance.set(list.filter(a => a.childId === id));
    });
  }

  setActiveTab(tab: 'enrollments' | 'payments' | 'attendance'): void {
    this.activeTab.set(tab);
  }
}
