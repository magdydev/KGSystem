import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { AcademicYear } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { AcademicYearFormComponent } from '../academic-year-form/academic-year-form.component';

@Component({
  selector: 'app-academic-year-list',
  standalone: true,
  imports: [AsyncPipe, DatePipe, TranslatePipe, ModalComponent, AcademicYearFormComponent],
  templateUrl: './academic-year-list.component.html',
  styleUrl: './academic-year-list.component.scss',
})
export class AcademicYearListComponent {
  private readonly referenceDataService = inject(ReferenceDataService);

  academicYears$: Observable<AcademicYear[]> = this.referenceDataService.getAcademicYears();

  readonly modalOpen = signal(false);
  readonly editYear = signal<AcademicYear | null>(null);

  get modalTitle(): string {
    return this.editYear() ? 'REFERENCE.ACADEMIC_YEARS.EDIT_TITLE' : 'REFERENCE.ACADEMIC_YEARS.ADD_TITLE';
  }

  openAdd(): void {
    this.editYear.set(null);
    this.modalOpen.set(true);
  }

  openEdit(year: AcademicYear): void {
    this.editYear.set(year);
    this.modalOpen.set(true);
  }

  onSaved(): void {
    this.modalOpen.set(false);
    this.academicYears$ = this.referenceDataService.getAcademicYears();
  }

  deleteAcademicYear(id: string): void {
    if (confirm('Are you sure you want to delete this academic year?')) {
      this.referenceDataService.deleteAcademicYear(id).subscribe(() => {
        this.academicYears$ = this.referenceDataService.getAcademicYears();
      });
    }
  }

  toggleActive(year: AcademicYear): void {
    const request = {
      code: year.code,
      nameAr: year.nameAr,
      nameEn: year.nameEn,
      startDate: year.startDate,
      endDate: year.endDate,
      isActive: !year.isActive,
    };
    this.referenceDataService.updateAcademicYear(year.id, request).subscribe(() => {
      this.academicYears$ = this.referenceDataService.getAcademicYears();
    });
  }
}
