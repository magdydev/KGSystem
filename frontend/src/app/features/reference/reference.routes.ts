import { Routes } from '@angular/router';

export const REFERENCE_ROUTES: Routes = [
  { path: '', redirectTo: 'phases', pathMatch: 'full' },
  {
    path: 'phases',
    loadComponent: () => import('./phase-list/phase-list.component').then(m => m.PhaseListComponent),
  },
  {
    path: 'academic-years',
    loadComponent: () => import('./academic-year-list/academic-year-list.component').then(m => m.AcademicYearListComponent),
  },
  {
    path: 'monthly-fees',
    loadComponent: () => import('./monthly-fee-list/monthly-fee-list.component').then(m => m.MonthlyFeeListComponent),
  },
];
