import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppHeaderComponent } from '../../components/app-header/app-header.component';
import { GlobalSpinnerComponent } from '../../components/global-spinner/global-spinner.component';
import { ToastComponent } from '../../components/toast/toast.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet, AppHeaderComponent, GlobalSpinnerComponent, ToastComponent],
  templateUrl: './auth-layout.component.html',
  styleUrl: './auth-layout.component.scss',
})
export class AuthLayoutComponent {}
