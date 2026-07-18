import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { AuthService } from '../../../core/services/auth.service';
import { LayoutService } from '../../../core/services/layout.service';

interface NavItem {
  label: string;
  route: string;
  icon: string;
  roles?: string[];
}

interface NavGroup {
  label: string | null;
  items: NavItem[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  protected readonly authService = inject(AuthService);
  protected readonly layoutService = inject(LayoutService);

  private readonly navGroups: NavGroup[] = [
    {
      label: null,
      items: [
        { label: 'NAV.DASHBOARD', route: '/dashboard', icon: 'dashboard', roles: ['Manager'] },
      ],
    },
    {
      label: 'NAV.GROUP_GENERAL',
      items: [
        { label: 'NAV.CHILDREN', route: '/children', icon: 'child_care' },
        { label: 'NAV.PAYMENTS', route: '/payments', icon: 'payments' },
        { label: 'NAV.PAYMENT_FOLLOW_UP', route: '/payments/follow-up', icon: 'campaign' },
        { label: 'NAV.ATTENDANCE', route: '/attendance', icon: 'fact_check' },
        { label: 'NAV.ATTENDANCE_HISTORY', route: '/attendance-report', icon: 'history' },
      ],
    },
    {
      label: 'NAV.GROUP_MANAGEMENT',
      items: [
        { label: 'NAV.PHASES', route: '/phases', icon: 'layers', roles: ['Manager'] },
        { label: 'NAV.ACADEMIC_YEARS', route: '/academic-years', icon: 'calendar_today', roles: ['Manager'] },
        { label: 'NAV.MONTHLY_FEES', route: '/monthly-fees', icon: 'attach_money', roles: ['Manager'] },
        { label: 'NAV.ARCHIVE', route: '/archive', icon: 'archive', roles: ['Manager'] },
      ],
    },
    {
      label: 'NAV.SETTINGS',
      items: [
        { label: 'NAV.USERS', route: '/users', icon: 'group', roles: ['Manager'] },
        { label: 'NAV.SETTINGS', route: '/settings', icon: 'settings', roles: ['Manager'] },
      ],
    },
  ];

  visibleGroups(): NavGroup[] {
    return this.navGroups
      .map(group => ({
        label: group.label,
        items: group.items.filter(item => !item.roles || item.roles.some(role => this.authService.hasRole(role))),
      }))
      .filter(group => group.items.length > 0);
  }
}
