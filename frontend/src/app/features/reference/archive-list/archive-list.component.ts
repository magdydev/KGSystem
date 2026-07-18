import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { Observable, map } from 'rxjs';
import { AcademicYear } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ArchiveYearDetailComponent } from '../archive-year-detail/archive-year-detail.component';

@Component({
  selector: 'app-archive-list',
  standalone: true,
  imports: [AsyncPipe, DatePipe, TranslatePipe, ModalComponent, ArchiveYearDetailComponent],
  templateUrl: './archive-list.component.html',
  styleUrl: './archive-list.component.scss',
})
export class ArchiveListComponent {
  private readonly referenceDataService = inject(ReferenceDataService);

  readonly archivedYears$: Observable<AcademicYear[]> = this.referenceDataService
    .getAcademicYears()
    .pipe(map(years => years.filter(y => !y.isActive)));

  readonly detailModalOpen = signal(false);
  readonly viewYear = signal<AcademicYear | null>(null);

  openDetail(year: AcademicYear): void {
    this.viewYear.set(year);
    this.detailModalOpen.set(true);
  }
}
