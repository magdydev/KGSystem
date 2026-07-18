import { Routes } from '@angular/router';

export const PAYMENTS_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./payment-list/payment-list.component').then(m => m.PaymentListComponent) },
  { path: 'follow-up', loadComponent: () => import('./payment-follow-up/payment-follow-up.component').then(m => m.PaymentFollowUpComponent) },
];
