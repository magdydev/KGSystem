import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { SystemUser } from '../../../core/models/user.model';
import { UserManagementService } from '../../../core/services/user-management.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
})
export class UserFormComponent implements OnInit {
  private readonly userManagementService = inject(UserManagementService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly editUser = input<SystemUser | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly saving = signal(false);
  readonly availableRoles = signal<string[]>([]);

  username = '';
  password = '';
  selectedRoles: string[] = [];

  ngOnInit(): void {
    const user = this.editUser();
    if (user) {
      this.username = user.userName;
      this.selectedRoles = [...user.roles];
    }

    this.userManagementService.getRoles().subscribe({
      next: roles => this.availableRoles.set(roles),
      error: () => this.toast.error(this.translate.instant('TOAST.LOAD_ERROR')),
    });
  }

  toggleRole(role: string, checked: boolean): void {
    this.selectedRoles = checked
      ? [...this.selectedRoles, role]
      : this.selectedRoles.filter(r => r !== role);
  }

  save(): void {
    this.saving.set(true);

    const done = () => { this.saving.set(false); this.saved.emit(); };
    const fail = (err: unknown) => {
      this.saving.set(false);
      const message = (err as { error?: { error?: string } })?.error?.error;
      this.toast.error(message || this.translate.instant('TOAST.SAVE_ERROR'));
    };

    const user = this.editUser();
    if (user) {
      this.userManagementService.updateUserRoles(user.id, this.selectedRoles).subscribe({ next: done, error: fail });
    } else {
      this.userManagementService.createUser({
        username: this.username,
        password: this.password,
        roles: this.selectedRoles,
      }).subscribe({ next: done, error: fail });
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
