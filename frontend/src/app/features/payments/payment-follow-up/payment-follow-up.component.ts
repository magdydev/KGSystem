import { CurrencyPipe, UpperCasePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { forkJoin } from 'rxjs';
import { PaymentService } from '../../../core/services/payment.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { ChildService } from '../../../core/services/child.service';
import { BrandingService } from '../../../core/services/branding.service';

interface FollowUpRow {
  paymentId: number;
  childNameAr: string;
  childNameEn: string;
  guardianNameAr: string;
  guardianNameEn: string;
  guardianPhone: string;
  month: number;
  year: number;
  amountDue: number;
  amountPaid: number;
  remaining: number;
  status: string;
}

const ARABIC_MONTHS = ['', 'يناير', 'فبراير', 'مارس', 'إبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];

@Component({
  selector: 'app-payment-follow-up',
  standalone: true,
  imports: [CurrencyPipe, UpperCasePipe, TranslatePipe],
  template: `
    <div class="page payment-follow-up">
      <div class="page-header">
        <h1 class="page-title">{{ 'PAYMENTS.FOLLOW_UP.TITLE' | translate }}</h1>
      </div>

      <div class="card" style="margin-bottom:1rem;padding:1rem 1.25rem;display:flex;gap:2rem;flex-wrap:wrap">
        <div><strong>{{ rows().length }}</strong> {{ 'PAYMENTS.FOLLOW_UP.OUTSTANDING_COUNT' | translate }}</div>
        <div><strong>{{ totalOutstanding() | currency: 'EGP' }}</strong> {{ 'PAYMENTS.FOLLOW_UP.TOTAL_OUTSTANDING' | translate }}</div>
      </div>

      <div class="card">
        @if (loading()) {
          <div class="loading">{{ 'COMMON.LOADING' | translate }}...</div>
        } @else if (rows().length === 0) {
          <div class="empty">{{ 'PAYMENTS.FOLLOW_UP.NO_OUTSTANDING' | translate }}</div>
        } @else {
          <table class="table">
            <thead>
              <tr>
                <th>{{ 'CHILDREN.NAME' | translate }}</th>
                <th>{{ 'PAYMENTS.FOLLOW_UP.GUARDIAN' | translate }}</th>
                <th>{{ 'PAYMENTS.FOLLOW_UP.PHONE' | translate }}</th>
                <th>{{ 'PAYMENTS.MONTH' | translate }}</th>
                <th>{{ 'PAYMENTS.REMAINING' | translate }}</th>
                <th>{{ 'PAYMENTS.STATUS' | translate }}</th>
                <th>{{ 'PAYMENTS.FOLLOW_UP.ACTION' | translate }}</th>
              </tr>
            </thead>
            <tbody>
              @for (row of rows(); track row.paymentId) {
                <tr>
                  <td><span style="font-weight:500">{{ row.childNameAr }} - {{ row.childNameEn }}</span></td>
                  <td>{{ row.guardianNameAr }} / {{ row.guardianNameEn }}</td>
                  <td dir="ltr" style="text-align:end">{{ row.guardianPhone }}</td>
                  <td>{{ 'MONTHS.' + row.month | translate }} {{ row.year }}</td>
                  <td style="font-weight:600;color:var(--color-danger)">{{ row.remaining | currency: 'EGP' }}</td>
                  <td>
                    <span class="badge" [class.badge-danger]="row.status === 'Unpaid'" [class.badge-warning]="row.status === 'Partial'">
                      {{ 'PAYMENTS.STATUS_' + (row.status | uppercase) | translate }}
                    </span>
                  </td>
                  <td>
                    @if (row.guardianPhone) {
                      <a class="btn btn-sm whatsapp-btn" [href]="whatsappLink(row)" target="_blank" rel="noopener">
                        <svg class="whatsapp-icon" viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                          <path d="M12.04 2c-5.52 0-10 4.48-10 10 0 1.77.46 3.45 1.27 4.9L2 22l5.25-1.38a9.96 9.96 0 0 0 4.79 1.22h.01c5.52 0 10-4.48 10-10s-4.48-10-10.01-10Zm0 18.15h-.01a8.2 8.2 0 0 1-4.18-1.15l-.3-.18-3.11.82.83-3.03-.2-.31a8.17 8.17 0 0 1-1.26-4.35c0-4.52 3.68-8.2 8.21-8.2 2.19 0 4.25.85 5.8 2.4a8.13 8.13 0 0 1 2.4 5.81c0 4.53-3.68 8.19-8.18 8.19Zm4.49-6.13c-.25-.12-1.45-.71-1.67-.79-.22-.08-.39-.12-.55.12-.16.25-.63.79-.78.95-.14.16-.29.18-.53.06-.25-.12-1.05-.39-2-1.23a7.49 7.49 0 0 1-1.38-1.72c-.14-.25-.02-.38.11-.5.11-.11.25-.29.37-.43.12-.14.16-.25.24-.41.08-.16.04-.31-.02-.43-.06-.12-.55-1.33-.76-1.82-.2-.48-.4-.42-.55-.42-.14-.01-.31-.01-.47-.01a.9.9 0 0 0-.65.31c-.22.25-.86.84-.86 2.05s.88 2.38 1 2.54c.12.16 1.73 2.64 4.19 3.7.59.25 1.04.4 1.4.51.59.19 1.12.16 1.54.1.47-.07 1.45-.59 1.65-1.16.2-.57.2-1.06.14-1.16-.06-.1-.22-.16-.47-.28Z"/>
                        </svg>
                        {{ 'PAYMENTS.FOLLOW_UP.SEND_REMINDER' | translate }}
                      </a>
                    } @else {
                      <span class="tab-empty" style="font-size:var(--text-xs)">{{ 'PAYMENTS.FOLLOW_UP.NO_PHONE' | translate }}</span>
                    }
                  </td>
                </tr>
              }
            </tbody>
          </table>
        }
      </div>
    </div>
  `,
  styles: [`
    .whatsapp-btn {
      background: #25d366;
      color: #fff;
      border-color: #25d366;
      text-decoration: none;
      display: inline-flex;
      align-items: center;
      gap: 0.35rem;

      &:hover {
        background: #1ebd59;
        border-color: #1ebd59;
      }
    }

    .whatsapp-icon {
      width: 16px;
      height: 16px;
      flex-shrink: 0;
    }
  `],
})
export class PaymentFollowUpComponent {
  private readonly paymentService = inject(PaymentService);
  private readonly enrollmentService = inject(EnrollmentService);
  private readonly childService = inject(ChildService);
  private readonly brandingService = inject(BrandingService);

  readonly loading = signal(true);
  readonly rows = signal<FollowUpRow[]>([]);

  readonly totalOutstanding = computed(() => this.rows().reduce((sum, r) => sum + r.remaining, 0));

  constructor() {
    forkJoin({
      unpaid: this.paymentService.getAll(undefined, undefined, 'Unpaid'),
      partial: this.paymentService.getAll(undefined, undefined, 'Partial'),
      enrollments: this.enrollmentService.getAll(),
      children: this.childService.getAll(),
    }).subscribe({
      next: ({ unpaid, partial, enrollments, children }) => {
        const enrollmentToChild = new Map(enrollments.map(e => [e.id, e.childId]));
        const childById = new Map(children.map(c => [c.id, c]));

        const rows: FollowUpRow[] = [...unpaid, ...partial]
          .map(payment => {
            const childId = enrollmentToChild.get(payment.enrollmentId);
            const child = childId ? childById.get(childId) : undefined;
            return {
              paymentId: payment.id,
              childNameAr: payment.childNameAr,
              childNameEn: payment.childNameEn,
              guardianNameAr: child?.guardianNameAr ?? '—',
              guardianNameEn: child?.guardianNameEn ?? '—',
              guardianPhone: child?.guardianPhone ?? '',
              month: payment.month,
              year: payment.year,
              amountDue: payment.amountDue,
              amountPaid: payment.amountPaid,
              remaining: payment.remaining,
              status: payment.status,
            };
          })
          .sort((a, b) => b.remaining - a.remaining);

        this.rows.set(rows);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  whatsappLink(row: FollowUpRow): string {
    const digits = row.guardianPhone.replace(/\D/g, '');
    const international = digits.startsWith('0') ? '2' + digits : digits;
    const appName = this.brandingService.branding().appNameAr || this.brandingService.branding().appName;
    const monthName = ARABIC_MONTHS[row.month] ?? row.month;
    const message =
      `مرحباً ${row.guardianNameAr}،\n` +
      `نود تذكيركم بأن رسوم ${row.childNameAr} الشهرية عن شهر ${monthName} ${row.year} ` +
      `بقيمة ${row.remaining} جنيه لا تزال مستحقة السداد.\n` +
      `يرجى التكرم بالسداد في أقرب وقت ممكن.\n` +
      `شكراً لتعاونكم - ${appName}`;
    return `https://wa.me/${international}?text=${encodeURIComponent(message)}`;
  }
}
