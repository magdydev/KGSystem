import { Component, input, output, model, HostListener } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  template: `
    @if (open()) {
      <div class="modal-overlay" (click)="onOverlayClick($event)">
        <div class="modal-panel" [style.max-width]="width()" role="dialog" [attr.aria-modal]="true">
          <div class="modal-header">
            <h2 class="modal-title">{{ title() }}</h2>
            <button type="button" class="modal-close" (click)="close()" aria-label="Close">&times;</button>
          </div>
          <div class="modal-body">
            <ng-content></ng-content>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      inset: 0;
      z-index: 1000;
      display: flex;
      align-items: center;
      justify-content: center;
      background: oklch(0% 0 0 / 40%);
      padding: 1rem;
      animation: fade-in 0.15s ease-out;
    }

    .modal-panel {
      background: var(--color-bg-white);
      border-radius: var(--radius-xl);
      box-shadow: 0 25px 50px oklch(0% 0 0 / 25%);
      width: 100%;
      max-height: 90dvh;
      display: flex;
      flex-direction: column;
      animation: slide-up 0.2s ease-out;
    }

    .modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1.25rem 1.5rem;
      background: color-mix(in srgb, var(--color-primary) 8%, var(--color-bg-white));
      border-bottom: 1px solid var(--color-border);
      border-radius: var(--radius-xl) var(--radius-xl) 0 0;
      flex-shrink: 0;
    }

    .modal-title {
      margin: 0;
      font-size: 1.125rem;
      font-weight: 700;
      color: var(--color-text);
    }

    .modal-close {
      border: none;
      background: transparent;
      font-size: 1.5rem;
      color: var(--color-text-muted);
      cursor: pointer;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: var(--radius-sm);
      transition: all var(--transition-fast);

      &:hover {
        background: var(--color-bg-subtle);
        color: var(--color-text);
      }
    }

    .modal-body {
      padding: 1.5rem;
      overflow-y: auto;
      flex: 1;
    }

    @keyframes fade-in {
      from { opacity: 0; }
      to   { opacity: 1; }
    }

    @keyframes slide-up {
      from { opacity: 0; transform: translateY(12px) scale(0.98); }
      to   { opacity: 1; transform: translateY(0) scale(1); }
    }
  `],
})
export class ModalComponent {
  readonly open = model(false);
  readonly title = input('');
  readonly width = input('560px');
  readonly closeOnOverlay = input(true);

  readonly closed = output<void>();

  @HostListener('document:keydown.escape')
  onEscape(): void {
    this.close();
  }

  onOverlayClick(event: MouseEvent): void {
    if (this.closeOnOverlay() && (event.target as HTMLElement).classList.contains('modal-overlay')) {
      this.close();
    }
  }

  close(): void {
    this.open.set(false);
    this.closed.emit();
  }
}
