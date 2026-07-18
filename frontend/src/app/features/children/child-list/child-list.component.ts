import { DatePipe, LowerCasePipe, UpperCasePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Subject, debounceTime } from 'rxjs';
import { ChildService } from '../../../core/services/child.service';
import { ToastService } from '../../../core/services/toast.service';
import { Child, ChildDetail } from '../../../core/models/child.model';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ChildFormComponent } from '../child-form/child-form.component';
import { ChildDetailComponent } from '../child-detail/child-detail.component';

@Component({
  selector: 'app-child-list',
  standalone: true,
  imports: [DatePipe, FormsModule, LowerCasePipe, TranslatePipe, UpperCasePipe, ModalComponent, ConfirmDialogComponent, ChildFormComponent, ChildDetailComponent],
  templateUrl: './child-list.component.html',
  styleUrl: './child-list.component.scss',
})
export class ChildListComponent {
  private readonly childService = inject(ChildService);
  private readonly translate = inject(TranslateService);
  private readonly toast = inject(ToastService);

  readonly statusOptions = ['Active', 'Inactive', 'Transferred', 'Graduated', 'Suspended'];
  selectedStatus = '';
  searchTerm = '';

  readonly children = signal<Child[]>([]);
  readonly loading = signal(true);

  readonly formModalOpen = signal(false);
  readonly detailModalOpen = signal(false);
  readonly editChild = signal<ChildDetail | null>(null);
  readonly viewChildId = signal<number>(0);

  readonly deleteConfirmOpen = signal(false);
  readonly childToDelete = signal<number | null>(null);

  private readonly searchSubject = new Subject<void>();

  constructor() {
    this.searchSubject.pipe(debounceTime(400)).subscribe(() => this.loadChildren());
    this.loadChildren();
  }

  private loadChildren(): void {
    this.loading.set(true);
    this.childService.getAll(this.searchTerm || undefined, this.selectedStatus || undefined).subscribe({
      next: children => {
        this.children.set(children);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toast.error(this.translate.instant('TOAST.LOAD_ERROR'));
      },
    });
  }

  onSearch(): void {
    this.searchSubject.next();
  }

  onStatusChange(): void {
    this.loadChildren();
  }

  openAdd(): void {
    this.editChild.set(null);
    this.formModalOpen.set(true);
  }

  openEdit(child: Child): void {
    this.childService.getById(child.id).subscribe(detail => {
      this.editChild.set(detail);
      this.formModalOpen.set(true);
    });
  }

  openView(id: number): void {
    this.viewChildId.set(id);
    this.detailModalOpen.set(true);
  }

  onFormSaved(): void {
    this.formModalOpen.set(false);
    this.loadChildren();
  }

  openDeleteConfirm(id: number): void {
    this.childToDelete.set(id);
    this.deleteConfirmOpen.set(true);
  }

  confirmDelete(): void {
    const id = this.childToDelete();
    if (id === null) return;

    this.childService.delete(id).subscribe({
      next: () => {
        this.loadChildren();
        this.toast.success(this.translate.instant('TOAST.CHILD_DELETED'));
      },
      error: () => this.toast.error(this.translate.instant('TOAST.CHILD_DELETE_ERROR')),
    });
  }
}
