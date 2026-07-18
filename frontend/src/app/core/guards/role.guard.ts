import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const userRoles = auth.getRoles();
  if (allowedRoles.some(role => userRoles.includes(role))) {
    return true;
  }

  return router.parseUrl('/dashboard');
};
