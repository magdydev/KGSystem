export interface BrandingSettings {
  appName: string;
  appNameAr: string;
  logoUrl: string | null;
  logoData: string | null;
  primaryColor: string;
  secondaryColor: string;
  currency: string;
}

export function logoSource(settings: BrandingSettings): string | null {
  return settings.logoData ?? settings.logoUrl;
}
