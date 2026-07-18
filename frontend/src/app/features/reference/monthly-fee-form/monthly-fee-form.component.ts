import { AsyncPipe } from '@angular/common';
import { Component, inject, input, output, signal, effect } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';
import { MonthlyFeeItem } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';

interface FeeRow {
  month: number;
  amount: number;
  currency: string;
  dueDate: string;
}

@Component({
  selector: 'app-monthly-fee-form',
  standalone: true,
  imports: [AsyncPipe, DateInputComponent, FormsModule, TranslatePipe],
  templateUrl: './monthly-fee-form.component.html',
  styleUrl: './monthly-fee-form.component.scss',
})
export class MonthlyFeeFormComponent {
  private readonly referenceDataService = inject(ReferenceDataService);

  readonly academicYearId = input.required<string>();
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly saving = signal(false);
  readonly loading = signal(true);

  rows: FeeRow[] = [];

  constructor() {
    effect(() => {
      const yearId = this.academicYearId();
      if (yearId) {
        this.loading.set(true);
        this.referenceDataService.getMonthlyFees().subscribe(fees => {
          const yearFees = fees.filter(f => f.academicYearId === yearId);
          this.rows = Array.from({ length: 12 }, (_, i) => {
            const month = i + 1;
            const existing = yearFees.find(f => f.month === month);
            return {
              month,
              amount: existing?.amount ?? 0,
              currency: existing?.currency ?? 'EGP',
              dueDate: existing?.dueDate ? existing.dueDate.substring(0, 10) : '',
            };
          });
          this.loading.set(false);
        });
      }
    });
  }

  save(): void {
    this.saving.set(true);
    const fees: MonthlyFeeItem[] = this.rows
      .filter(r => r.amount > 0)
      .map(r => ({
        month: r.month,
        amount: r.amount,
        currency: r.currency,
        dueDate: r.dueDate || null,
      }));

    this.referenceDataService.patchMonthlyFeesByYear({
      academicYearId: this.academicYearId(),
      fees,
    }).subscribe({
      next: () => { this.saving.set(false); this.saved.emit(); },
      error: () => this.saving.set(false),
    });
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
