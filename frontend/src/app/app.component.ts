import { Component, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs';
import { LanguageService } from './core/services/language.service';
import { AuthService } from './core/services/auth.service';
import { AuthLayoutComponent } from './shared/layouts/auth-layout/auth-layout.component';
import { MainLayoutComponent } from './shared/layouts/main-layout/main-layout.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [AuthLayoutComponent, MainLayoutComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  private readonly languageService = inject(LanguageService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly isAuthRoute = signal(this.isAuthPath(this.router.url));

  constructor() {
    this.languageService.init();
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
    ).subscribe(e => {
      this.isAuthRoute.set(this.isAuthPath((e as NavigationEnd).url));
    });
  }

  private isAuthPath(url: string): boolean {
    return url.startsWith('/auth') || !this.authService.isLoggedIn();
  }
}
