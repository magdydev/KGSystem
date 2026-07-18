import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        router.navigate(['/auth/login']);
      } else if (error.status === 0) {
        console.error('Network error — is the API reachable?', error);
      } else {
        console.error(`API error ${error.status} on ${req.method} ${req.url}`, error.error);
      }

      return throwError(() => error);
    }),
  );
};
