import { Routes } from '@angular/router';

export const CHILDREN_ROUTES: Routes = [
  { path: '', loadComponent: () => import('./child-list/child-list.component').then(m => m.ChildListComponent) },
];
