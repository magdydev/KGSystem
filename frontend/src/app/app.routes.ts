import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard, () => roleGuard(['Manager'])],
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
  },
  {
    path: 'children',
    canActivate: [authGuard],
    loadChildren: () => import('./features/children/children.routes').then(m => m.CHILDREN_ROUTES),
  },
  {
    path: 'payments',
    canActivate: [authGuard],
    loadChildren: () => import('./features/payments/payments.routes').then(m => m.PAYMENTS_ROUTES),
  },
  {
    path: 'attendance',
    canActivate: [authGuard],
    loadChildren: () => import('./features/attendance/attendance.routes').then(m => m.ATTENDANCE_ROUTES),
  },
  {
    path: 'attendance-report',
    canActivate: [authGuard],
    loadComponent: () => import('./features/attendance/attendance-history/attendance-history.component').then(m => m.AttendanceHistoryComponent),
  },
  {
    path: 'phases',
    canActivate: [authGuard, () => roleGuard(['Manager'])],
    loadComponent: () => import('./features/reference/phase-list/phase-list.component').then(m => m.PhaseListComponent),
  },
  {
    path: 'academic-years',
    canActivate: [authGuard, () => roleGuard(['Manager'])],
    loadComponent: () => import('./features/reference/academic-year-list/academic-year-list.component').then(m => m.AcademicYearListComponent),
  },
  {
    path: 'monthly-fees',
    canActivate: [authGuard, () => roleGuard(['Manager'])],
    loadComponent: () => import('./features/reference/monthly-fee-list/monthly-fee-list.component').then(m => m.MonthlyFeeListComponent),
  },
  {
    path: 'settings',
    canActivate: [authGuard, () => roleGuard(['Manager'])],
    loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent),
  },
  { path: '**', redirectTo: 'dashboard' },
];
