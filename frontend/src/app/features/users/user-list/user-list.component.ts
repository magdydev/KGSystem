import { Component, inject, signal } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';
import { UserManagementService } from '../../../core/services/user-management.service';
import { ToastService } from '../../../core/services/toast.service';
import { SystemUser } from '../../../core/models/user.model';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { UserFormComponent } from '../user-form/user-form.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [TranslatePipe, ModalComponent, ConfirmDialogComponent, UserFormComponent],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss',
})
export class UserListComponent {
  private readonly userManagementService = inject(UserManagementService);
  private readonly authService = inject(AuthService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly users = signal<SystemUser[]>([]);
  readonly loading = signal(true);

  readonly modalOpen = signal(false);
  readonly editUser = signal<SystemUser | null>(null);

  readonly deleteConfirmOpen = signal(false);
  readonly userToDelete = signal<SystemUser | null>(null);

  get modalTitle(): string {
    return this.editUser() ? 'USERS.EDIT_ROLES_TITLE' : 'USERS.ADD_TITLE';
  }

  constructor() {
    this.loadUsers();
  }

  private loadUsers(): void {
    this.loading.set(true);
    this.userManagementService.getUsers().subscribe({
      next: users => {
        this.users.set(users);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toast.error(this.translate.instant('TOAST.LOAD_ERROR'));
      },
    });
  }

  isCurrentUser(user: SystemUser): boolean {
    return this.authService.userEmail() === user.email;
  }

  openAdd(): void {
    this.editUser.set(null);
    this.modalOpen.set(true);
  }

  openEditRoles(user: SystemUser): void {
    this.editUser.set(user);
    this.modalOpen.set(true);
  }

  onSaved(): void {
    this.modalOpen.set(false);
    this.loadUsers();
  }

  openDeleteConfirm(user: SystemUser): void {
    this.userToDelete.set(user);
    this.deleteConfirmOpen.set(true);
  }

  confirmDelete(): void {
    const user = this.userToDelete();
    if (!user) return;

    this.userManagementService.deleteUser(user.id).subscribe({
      next: () => {
        this.loadUsers();
        this.toast.success(this.translate.instant('TOAST.USER_DELETED'));
      },
      error: (err: unknown) => {
        const message = (err as { error?: { error?: string } })?.error?.error;
        this.toast.error(message || this.translate.instant('TOAST.USER_DELETE_ERROR'));
      },
    });
  }
}
