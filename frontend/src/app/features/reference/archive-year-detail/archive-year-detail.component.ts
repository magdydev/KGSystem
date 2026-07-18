import { CurrencyPipe, DatePipe, UpperCasePipe } from '@angular/common';
import { Component, OnInit, inject, input, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ArchivedYearDetail } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-archive-year-detail',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, UpperCasePipe, TranslatePipe],
  templateUrl: './archive-year-detail.component.html',
  styleUrl: './archive-year-detail.component.scss',
})
export class ArchiveYearDetailComponent implements OnInit {
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly yearId = input.required<number>();

  readonly activeTab = signal<'enrollments' | 'payments'>('enrollments');
  readonly detail = signal<ArchivedYearDetail | null>(null);
  readonly loading = signal(true);

  ngOnInit(): void {
    this.referenceDataService.getArchivedYearDetail(this.yearId()).subscribe({
      next: detail => {
        this.detail.set(detail);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toast.error(this.translate.instant('TOAST.LOAD_ERROR'));
      },
    });
  }

  setActiveTab(tab: 'enrollments' | 'payments'): void {
    this.activeTab.set(tab);
  }
}
