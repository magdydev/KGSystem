import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CreateAcademicYearRequest, AcademicYear } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';

@Component({
  selector: 'app-academic-year-form',
  standalone: true,
  imports: [FormsModule, TranslatePipe, DateInputComponent],
  templateUrl: './academic-year-form.component.html',
  styleUrl: './academic-year-form.component.scss',
})
export class AcademicYearFormComponent implements OnInit {
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly editYear = input<AcademicYear | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly saving = signal(false);

  code = '';
  nameAr = '';
  nameEn = '';
  startDate = '';
  endDate = '';
  isActive = false;

  ngOnInit(): void {
    const year = this.editYear();
    if (year) {
      this.code = year.code;
      this.nameAr = year.nameAr;
      this.nameEn = year.nameEn;
      this.startDate = year.startDate;
      this.endDate = year.endDate;
      this.isActive = year.isActive;
    }
  }

  save(): void {
    this.saving.set(true);
    const request: CreateAcademicYearRequest = {
      code: this.code,
      nameAr: this.nameAr,
      nameEn: this.nameEn,
      startDate: this.startDate,
      endDate: this.endDate,
      isActive: this.isActive,
    };

    const done = () => { this.saving.set(false); this.saved.emit(); };
    const fail = () => {
      this.saving.set(false);
      this.toast.error(this.translate.instant('TOAST.SAVE_ERROR'));
    };

    if (this.editYear()) {
      this.referenceDataService.updateAcademicYear(this.editYear()!.id, request).subscribe({ next: done, error: fail });
    } else {
      this.referenceDataService.createAcademicYear(request).subscribe({ next: done, error: fail });
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
