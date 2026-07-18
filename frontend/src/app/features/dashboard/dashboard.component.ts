import { AsyncPipe, NgClass, CurrencyPipe, UpperCasePipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardSummary } from '../../core/models/dashboard.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [AsyncPipe, NgClass, CurrencyPipe, TranslatePipe, UpperCasePipe],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent {
  private readonly dashboardService = inject(DashboardService);
  private readonly translate = inject(TranslateService);

  readonly summary$: Observable<DashboardSummary> = this.dashboardService.getSummary();

  childName(p: { childNameAr: string; childNameEn: string }): string {
    return this.translate.currentLang() === 'ar' ? p.childNameAr : p.childNameEn;
  }

  phaseName(p: { phaseNameAr: string; phaseNameEn: string }): string {
    return this.translate.currentLang() === 'ar' ? p.phaseNameAr : p.phaseNameEn;
  }
}
