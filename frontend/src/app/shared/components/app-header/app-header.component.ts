import { Component, computed, inject } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { AppLanguage, LanguageService } from '../../../core/services/language.service';
import { AuthService } from '../../../core/services/auth.service';
import { BrandingService } from '../../../core/services/branding.service';
import { logoSource } from '../../../core/models/branding.model';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './app-header.component.html',
  styleUrl: './app-header.component.scss',
})
export class AppHeaderComponent {
  protected readonly languageService = inject(LanguageService);
  protected readonly brandingService = inject(BrandingService);
  protected readonly authService = inject(AuthService);

  protected readonly logoSrc = computed(() => logoSource(this.brandingService.branding()));

  switchTo(lang: AppLanguage): void {
    this.languageService.use(lang);
  }

  logout(): void {
    this.authService.logout();
  }
}
