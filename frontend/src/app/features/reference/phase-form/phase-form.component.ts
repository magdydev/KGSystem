import { Component, inject, input, output, signal, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { CreateKGPhaseRequest, KGPhase } from '../../../core/models/reference.model';
import { ReferenceDataService } from '../../../core/services/reference-data.service';

@Component({
  selector: 'app-phase-form',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './phase-form.component.html',
  styleUrl: './phase-form.component.scss',
})
export class PhaseFormComponent implements OnInit {
  private readonly referenceDataService = inject(ReferenceDataService);

  readonly editPhase = input<KGPhase | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly saving = signal(false);

  code = '';
  nameAr = '';
  nameEn = '';
  descriptionAr = '';
  descriptionEn = '';
  sortOrder = 0;

  ngOnInit(): void {
    const phase = this.editPhase();
    if (phase) {
      this.code = phase.code;
      this.nameAr = phase.nameAr;
      this.nameEn = phase.nameEn;
      this.descriptionAr = phase.descriptionAr ?? '';
      this.descriptionEn = phase.descriptionEn ?? '';
      this.sortOrder = phase.sortOrder;
    }
  }

  save(): void {
    this.saving.set(true);
    const request: CreateKGPhaseRequest = {
      code: this.code.toUpperCase(),
      nameAr: this.nameAr,
      nameEn: this.nameEn,
      descriptionAr: this.descriptionAr || undefined,
      descriptionEn: this.descriptionEn || undefined,
      sortOrder: this.sortOrder,
    };

    const done = () => { this.saving.set(false); this.saved.emit(); };
    const fail = () => this.saving.set(false);

    if (this.editPhase()) {
      this.referenceDataService.updatePhase(this.editPhase()!.id, request).subscribe({ next: done, error: fail });
    } else {
      this.referenceDataService.createPhase(request).subscribe({ next: done, error: fail });
    }
  }

  cancel(): void {
    this.cancelled.emit();
  }
}
