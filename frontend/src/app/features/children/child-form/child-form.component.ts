import { AsyncPipe } from '@angular/common';
import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { CreateChildRequest, ChildDetail } from '../../../core/models/child.model';
import { KGPhase, AcademicYear } from '../../../core/models/reference.model';
import { ChildService } from '../../../core/services/child.service';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';

@Component({
  selector: 'app-child-form',
  standalone: true,
  imports: [AsyncPipe, FormsModule, TranslatePipe, DateInputComponent],
  templateUrl: './child-form.component.html',
  styleUrl: './child-form.component.scss',
})
export class ChildFormComponent {
  private readonly childService = inject(ChildService);
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly editChild = input<ChildDetail | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly loading = signal(false);
  readonly saving = signal(false);

  readonly phases$: Observable<KGPhase[]> = this.referenceDataService.getPhases();
  readonly academicYears$: Observable<AcademicYear[]> = this.referenceDataService.getAcademicYears();

  formData: CreateChildRequest = {
    firstNameAr: '',
    firstNameEn: '',
    lastNameAr: '',
    lastNameEn: '',
    dateOfBirth: '',
    gender: 'Male',
    guardianNameAr: '',
    guardianNameEn: '',
    guardianPhone: '',
    guardianEmail: '',
    nationality: '',
    address: '',
    kgPhaseId: undefined,
    academicYearId: undefined,
  };

  constructor() {
    const child = this.editChild();
    if (child) {
      this.formData = {
        firstNameAr: child.firstNameAr,
        firstNameEn: child.firstNameEn,
        lastNameAr: child.lastNameAr,
        lastNameEn: child.lastNameEn,
        dateOfBirth: child.dateOfBirth,
        gender: child.gender,
        guardianNameAr: child.guardianNameAr,
        guardianNameEn: child.guardianNameEn,
        guardianPhone: child.guardianPhone,
        guardianEmail: child.guardianEmail ?? '',
        nationality: child.nationality ?? '',
        address: child.address ?? '',
        kgPhaseId: undefined,
        academicYearId: undefined,
      };
    }
  }

  save(): void {
    this.saving.set(true);

    const payload = {
      ...this.formData,
      kgPhaseId: this.formData.kgPhaseId || undefined,
      academicYearId: this.formData.academicYearId || undefined,
    };

    const fail = () => {
      this.saving.set(false);
      this.toast.error(this.translate.instant('TOAST.SAVE_ERROR'));
    };

    if (this.editChild()) {
      this.childService.update(this.editChild()!.id, payload).subscribe({
        next: () => { this.saving.set(false); this.saved.emit(); },
        error: fail,
      });
    } else {
      this.childService.create(payload).subscribe({
        next: () => { this.saving.set(false); this.saved.emit(); },
        error: fail,
      });
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
