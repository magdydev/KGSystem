import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppHeaderComponent } from '../../components/app-header/app-header.component';
import { AppFooterComponent } from '../../components/app-footer/app-footer.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { GlobalSpinnerComponent } from '../../components/global-spinner/global-spinner.component';
import { ToastComponent } from '../../components/toast/toast.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, AppHeaderComponent, AppFooterComponent, SidebarComponent, GlobalSpinnerComponent, ToastComponent],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss',
})
export class MainLayoutComponent {}
