import { Component, input, model, output } from '@angular/core';
import { ModalComponent } from '../modal/modal.component';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [ModalComponent],
  templateUrl: './confirm-dialog.component.html',
  styleUrl: './confirm-dialog.component.scss',
})
export class ConfirmDialogComponent {
  readonly open = model(false);
  readonly title = input('');
  readonly message = input('');
  readonly confirmText = input('Confirm');
  readonly cancelText = input('Cancel');
  readonly variant = input<'warning' | 'danger'>('warning');

  readonly confirmed = output<void>();

  confirm(): void {
    this.open.set(false);
    this.confirmed.emit();
  }

  cancel(): void {
    this.open.set(false);
  }
}
