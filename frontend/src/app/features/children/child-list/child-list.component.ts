import { DatePipe, LowerCasePipe, UpperCasePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Subject, debounceTime } from 'rxjs';
import { ChildService } from '../../../core/services/child.service';
import { Child, ChildDetail } from '../../../core/models/child.model';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ChildFormComponent } from '../child-form/child-form.component';
import { ChildDetailComponent } from '../child-detail/child-detail.component';

@Component({
  selector: 'app-child-list',
  standalone: true,
  imports: [DatePipe, FormsModule, LowerCasePipe, TranslatePipe, UpperCasePipe, ModalComponent, ChildFormComponent, ChildDetailComponent],
  templateUrl: './child-list.component.html',
  styleUrl: './child-list.component.scss',
})
export class ChildListComponent {
  private readonly childService = inject(ChildService);
  private readonly translate = inject(TranslateService);

  readonly statusOptions = ['Active', 'Inactive', 'Transferred', 'Graduated', 'Suspended'];
  selectedStatus = '';
  searchTerm = '';

  readonly children = signal<Child[]>([]);
  readonly loading = signal(true);

  readonly formModalOpen = signal(false);
  readonly detailModalOpen = signal(false);
  readonly editChild = signal<ChildDetail | null>(null);
  readonly viewChildId = signal<string>('');

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
      error: () => this.loading.set(false),
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

  openView(id: string): void {
    this.viewChildId.set(id);
    this.detailModalOpen.set(true);
  }

  onFormSaved(): void {
    this.formModalOpen.set(false);
    this.loadChildren();
  }

  deleteChild(id: string): void {
    if (confirm(this.translate.instant('CHILDREN.DELETE_CONFIRM'))) {
      this.childService.delete(id).subscribe(() => this.loadChildren());
    }
  }
}
