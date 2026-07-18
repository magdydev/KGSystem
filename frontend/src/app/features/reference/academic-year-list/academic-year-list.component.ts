import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { AcademicYear } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AcademicYearFormComponent } from '../academic-year-form/academic-year-form.component';

@Component({
  selector: 'app-academic-year-list',
  standalone: true,
  imports: [AsyncPipe, DatePipe, RouterLink, TranslatePipe, ModalComponent, ConfirmDialogComponent, AcademicYearFormComponent],
  templateUrl: './academic-year-list.component.html',
  styleUrl: './academic-year-list.component.scss',
})
export class AcademicYearListComponent {
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  academicYears$: Observable<AcademicYear[]> = this.referenceDataService.getAcademicYears();

  readonly modalOpen = signal(false);
  readonly editYear = signal<AcademicYear | null>(null);

  readonly archiveConfirmOpen = signal(false);
  readonly yearToArchive = signal<AcademicYear | null>(null);

  readonly deleteConfirmOpen = signal(false);
  readonly yearToDelete = signal<number | null>(null);

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

  openDeleteConfirm(id: number): void {
    this.yearToDelete.set(id);
    this.deleteConfirmOpen.set(true);
  }

  confirmDelete(): void {
    const id = this.yearToDelete();
    if (id === null) return;

    this.referenceDataService.deleteAcademicYear(id).subscribe({
      next: () => {
        this.academicYears$ = this.referenceDataService.getAcademicYears();
        this.toast.success(this.translate.instant('TOAST.ACADEMIC_YEAR_DELETED'));
      },
      error: () => this.toast.error(this.translate.instant('TOAST.ACADEMIC_YEAR_DELETE_ERROR')),
    });
  }

  openArchiveConfirm(year: AcademicYear): void {
    this.yearToArchive.set(year);
    this.archiveConfirmOpen.set(true);
  }

  confirmArchive(): void {
    const year = this.yearToArchive();
    if (!year) return;

    const request = {
      code: year.code,
      nameAr: year.nameAr,
      nameEn: year.nameEn,
      startDate: year.startDate,
      endDate: year.endDate,
      isActive: false,
    };
    this.referenceDataService.updateAcademicYear(year.id, request).subscribe({
      next: () => {
        this.academicYears$ = this.referenceDataService.getAcademicYears();
        this.toast.success(this.translate.instant('TOAST.ACADEMIC_YEAR_UPDATED'));
      },
      error: () => this.toast.error(this.translate.instant('TOAST.ACADEMIC_YEAR_UPDATE_ERROR')),
    });
  }
}
