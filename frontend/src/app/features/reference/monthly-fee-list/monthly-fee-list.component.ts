import { AsyncPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable, first } from 'rxjs';
import { AcademicYear, MonthlyFeeItem } from '../../../core/models/reference.model';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';

interface FeeRow {
  month: number;
  amount: number;
  dueDate: string;
}

@Component({
  selector: 'app-monthly-fee-list',
  standalone: true,
  imports: [AsyncPipe, CurrencyPipe, DateInputComponent, DatePipe, FormsModule, TranslatePipe],
  templateUrl: './monthly-fee-list.component.html',
  styleUrl: './monthly-fee-list.component.scss',
})
export class MonthlyFeeListComponent {
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);
  private academicYears: AcademicYear[] = [];

  readonly academicYears$: Observable<AcademicYear[]> = this.referenceDataService.getAcademicYears();
  readonly selectedYearId = signal<number>(0);
  readonly saving = signal(false);
  readonly loading = signal(false);

  rows: FeeRow[] = [];

  constructor() {
    this.academicYears$.pipe(first()).subscribe(years => {
      this.academicYears = years;
      if (years.length > 0) {
        this.onYearChange(years[0].id);
      }
    });
  }

  onYearChange(yearId: number): void {
    this.selectedYearId.set(yearId);
    if (!yearId) { this.rows = []; return; }

    const year = this.academicYears.find(y => y.id === yearId);
    const startYear = year?.startDate ? new Date(year.startDate).getFullYear() : new Date().getFullYear();
    const startMonth = year?.startDate ? new Date(year.startDate).getMonth() + 1 : 1;

    this.loading.set(true);
    this.referenceDataService.getMonthlyFees().subscribe(all => {
      const yearFees = all.filter(f => f.academicYearId === yearId);
      this.rows = Array.from({ length: 12 }, (_, i) => {
        const month = i + 1;
        const existing = yearFees.find(f => f.month === month);
        const calYear = month >= startMonth ? startYear : startYear + 1;
        const defaultDate = `${calYear}-${String(month).padStart(2, '0')}-01`;
        return {
          month,
          amount: existing?.amount ?? 0,
          dueDate: existing?.dueDate ? existing.dueDate.substring(0, 10) : defaultDate,
        };
      });
      this.loading.set(false);
    });
  }

  save(): void {
    this.saving.set(true);
    const yearId = this.selectedYearId();
    const fees: MonthlyFeeItem[] = this.rows
      .filter(r => r.amount > 0)
      .map(r => ({
        month: r.month,
        amount: r.amount,
        dueDate: r.dueDate || null,
      }));

    this.referenceDataService.patchMonthlyFeesByYear({
      academicYearId: yearId,
      fees,
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.onYearChange(yearId);
        this.toast.success(this.translate.instant('TOAST.MONTHLY_FEES_SAVED'));
      },
      error: () => {
        this.saving.set(false);
        this.toast.error(this.translate.instant('TOAST.MONTHLY_FEES_SAVE_ERROR'));
      },
    });
  }
}
