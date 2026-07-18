import { AsyncPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { KGPhase } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { PhaseFormComponent } from '../phase-form/phase-form.component';

@Component({
  selector: 'app-phase-list',
  standalone: true,
  imports: [AsyncPipe, TranslatePipe, ModalComponent, ConfirmDialogComponent, PhaseFormComponent],
  templateUrl: './phase-list.component.html',
  styleUrl: './phase-list.component.scss',
})
export class PhaseListComponent {
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  phases$: Observable<KGPhase[]> = this.referenceDataService.getPhases();

  readonly modalOpen = signal(false);
  readonly editPhase = signal<KGPhase | null>(null);

  readonly deleteConfirmOpen = signal(false);
  readonly phaseToDelete = signal<number | null>(null);

  get modalTitle(): string {
    return this.editPhase() ? 'REFERENCE.PHASES.EDIT_TITLE' : 'REFERENCE.PHASES.ADD_TITLE';
  }

  openAdd(): void {
    this.editPhase.set(null);
    this.modalOpen.set(true);
  }

  openEdit(phase: KGPhase): void {
    this.editPhase.set(phase);
    this.modalOpen.set(true);
  }

  onSaved(): void {
    this.modalOpen.set(false);
    this.phases$ = this.referenceDataService.getPhases();
  }

  openDeleteConfirm(id: number): void {
    this.phaseToDelete.set(id);
    this.deleteConfirmOpen.set(true);
  }

  confirmDelete(): void {
    const id = this.phaseToDelete();
    if (id === null) return;

    this.referenceDataService.deletePhase(id).subscribe({
      next: () => {
        this.phases$ = this.referenceDataService.getPhases();
        this.toast.success(this.translate.instant('TOAST.PHASE_DELETED'));
      },
      error: () => this.toast.error(this.translate.instant('TOAST.PHASE_DELETE_ERROR')),
    });
  }
}
