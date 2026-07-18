import { HttpClient } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { BrandingSettings } from '../models/branding.model';

const DEFAULT_BRANDING: BrandingSettings = {
  appName: 'KGSystem',
  appNameAr: 'نظام رياض الأطفال',
  logoUrl: 'assets/logo.svg',
  logoData: null,
  primaryColor: '#6366f1',
  secondaryColor: '#f59e0b',
  currency: 'EGP',
};

@Injectable({ providedIn: 'root' })
export class BrandingService {
  private readonly http = inject(HttpClient);
  private readonly titleService = inject(Title);

  private readonly _branding = signal<BrandingSettings>(DEFAULT_BRANDING);
  readonly branding = this._branding.asReadonly();

  async load(): Promise<void> {
    try {
      const settings = await firstValueFrom(
        this.http.get<BrandingSettings>(`${environment.apiBaseUrl}/v1/settings/branding`),
      );
      this._branding.set(settings);
    } catch {
      // API unreachable or not seeded yet — keep defaults
    }
    this.applyToDocument();
  }

  async update(settings: BrandingSettings): Promise<void> {
    const updated = await firstValueFrom(
      this.http.put<BrandingSettings>(`${environment.apiBaseUrl}/v1/settings/branding`, settings),
    );
    this._branding.set(updated);
    this.applyToDocument();
  }

  private applyToDocument(): void {
    const branding = this._branding();
    this.titleService.setTitle(branding.appName);

    const root = document.documentElement.style;
    root.setProperty('--color-primary', branding.primaryColor);
    root.setProperty('--color-secondary', branding.secondaryColor);
    root.setProperty('--color-hover', branding.secondaryColor);
  }
}
