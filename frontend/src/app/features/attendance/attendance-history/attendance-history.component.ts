import { Component, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Attendance } from '../../../core/models/attendance.model';
import { KGPhase } from '../../../core/models/reference.model';
import { Enrollment } from '../../../core/models/enrollment.model';
import { Child } from '../../../core/models/child.model';
import { AttendanceService } from '../../../core/services/attendance.service';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { ChildService } from '../../../core/services/child.service';
import { BrandingService } from '../../../core/services/branding.service';
import { ToastService } from '../../../core/services/toast.service';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';
import { forkJoin, firstValueFrom } from 'rxjs';


@Component({
  selector: 'app-attendance-history',
  standalone: true,
  imports: [TranslatePipe, DatePipe, FormsModule, DateInputComponent],
  template: `
    <div class="page attendance-history">
      <div class="page-header">
        <h1 class="page-title">{{ 'ATTENDANCE.REPORT_TITLE' | translate }}</h1>
      </div>

      <nav class="tab-nav">
        <button class="tab-btn" [class.active]="activeTab() === 'register'" (click)="activeTab.set('register')">
          {{ 'ATTENDANCE.SHOW_HISTORY' | translate }}
        </button>
        <button class="tab-btn" [class.active]="activeTab() === 'child-report'" (click)="activeTab.set('child-report')">
          {{ 'ATTENDANCE.CHILD_REPORT' | translate }}
        </button>
      </nav>

      @if (activeTab() === 'register') {
        <div class="tab-content">
          <div class="filters" style="display:flex;gap:1rem;flex-wrap:wrap;align-items:flex-end;margin-bottom:1rem">
            <div class="form-group">
              <label>{{ 'COMMON.YEAR' | translate }}</label>
              <select class="form-control" [ngModel]="selectedYear" (ngModelChange)="selectedYear = +$event; applyFilters()" style="width:auto">
                @for (y of years; track y) {
                  <option [ngValue]="y">{{ y }}</option>
                }
              </select>
            </div>
            <div class="form-group">
              <label>{{ 'COMMON.MONTH' | translate }}</label>
              <select class="form-control" [ngModel]="selectedMonth" (ngModelChange)="selectedMonth = +$event; applyFilters()" style="width:auto">
                <option [ngValue]="0">{{ 'COMMON.ALL' | translate }}</option>
                @for (m of [1,2,3,4,5,6,7,8,9,10,11,12]; track m) {
                  <option [ngValue]="m">{{ 'MONTHS.' + m | translate }}</option>
                }
              </select>
            </div>
            <div class="form-group">
              <label>{{ 'REFERENCE.PHASES.TITLE' | translate }}</label>
              <select class="form-control" [ngModel]="selectedPhaseId" (ngModelChange)="selectedPhaseId = +$event; applyFilters()" style="width:auto">
                <option value="">{{ 'COMMON.ALL' | translate }}</option>
                @for (p of phases; track p.id) {
                  <option [value]="p.id">{{ currentLang() === 'ar' ? p.nameAr : p.nameEn }}</option>
                }
              </select>
            </div>
            @if (groupedDates().length > 0) {
              <div class="form-group" style="align-self:flex-end">
                <button type="button" class="btn btn-primary btn-sm" (click)="printReport()">
                  <span class="material-symbols-outlined" style="font-size:16px">print</span>
                  {{ 'COMMON.PRINT' | translate }}
                </button>
              </div>
            }
          </div>

          <div class="card">
            @if (loading) {
              <div class="loading">{{ 'COMMON.LOADING' | translate }}...</div>
            } @else if (groupedDates().length === 0) {
              <div class="empty">{{ 'ATTENDANCE.NO_HISTORY' | translate }}</div>
            } @else {
              @for (dateKey of groupedDates(); track dateKey) {
                <div class="history-date-group">
                  <div class="history-date-header">
                    <span (click)="toggleDate(dateKey)" style="cursor:pointer;flex:1">{{ dateKey | date:'mediumDate' }}</span>
                    <span class="history-count">{{ dateGroups().get(dateKey)?.length }} {{ 'CHILDREN.TITLE' | translate }}</span>
                  </div>
                  @if (expandedDates().has(dateKey)) {
                    <table class="table">
                      <thead>
                        <tr>
                          <th>{{ 'CHILDREN.NAME' | translate }}</th>
                          <th>{{ 'ATTENDANCE.STATUS' | translate }}</th>
                          <th>{{ 'COMMON.NOTES' | translate }}</th>
                        </tr>
                      </thead>
                      <tbody>
                        @for (rec of dateGroups().get(dateKey); track rec.id) {
                          <tr>
                            <td>{{ rec.childNameAr }} - {{ rec.childNameEn }}</td>
                            <td>{{ 'ATTENDANCE.' + rec.status.toUpperCase() | translate }}</td>
                            <td>{{ rec.notes }}</td>
                          </tr>
                        }
                      </tbody>
                    </table>
                  }
                </div>
              }
            }
          </div>
        </div>
      }

      @if (activeTab() === 'child-report') {
        <div class="tab-content">
          <div class="filters" style="display:flex;gap:1rem;flex-wrap:wrap;align-items:flex-end;margin-bottom:1rem">
            <div class="form-group" style="flex:1;min-width:250px;position:relative" #childSearchWrapper>
              <label>{{ 'ATTENDANCE.SELECT_CHILD' | translate }}</label>
              <input type="text" class="form-control"
                [placeholder]="'ATTENDANCE.SEARCH_CHILD' | translate"
                [ngModel]="childSearchTerm()"
                (ngModelChange)="onChildSearch($event)"
                (focus)="childSearchOpen.set(true)"
                (blur)="onChildSearchBlur()" />
              @if (selectedChild(); as child) {
                <div class="selected-child-tag">
                  <span>{{ child.fullNameAr }} - {{ child.fullNameEn }}</span>
                  <button type="button" class="clear-btn" (click)="clearSelectedChild()">&times;</button>
                </div>
              }
              @if (childSearchOpen() && childSearchResults().length > 0) {
                <div class="search-dropdown">
                  @for (c of childSearchResults(); track c.id) {
                    <div class="search-dropdown-item" [class.highlighted]="selectedChild()?.id === c.id" (mousedown)="selectChild(c)">
                      <span>{{ c.fullNameAr }} - {{ c.fullNameEn }}</span>
                      <small>{{ c.status }}</small>
                    </div>
                  }
                </div>
              }
            </div>
            <div class="form-group">
              <label>{{ 'ATTENDANCE.DATE_FROM' | translate }}</label>
              <app-date-input [(ngModel)]="dateFrom" />
            </div>
            <div class="form-group">
              <label>{{ 'ATTENDANCE.DATE_TO' | translate }}</label>
              <app-date-input [(ngModel)]="dateTo" />
            </div>
            <div class="form-group" style="align-self:flex-end">
              <button type="button" class="btn btn-primary" (click)="generateChildReport()" [disabled]="reportLoading">
                <span class="material-symbols-outlined" style="font-size:18px">summarize</span>
                {{ 'ATTENDANCE.GENERATE_REPORT' | translate }}
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .history-date-group {
      border-bottom: 1px solid var(--color-border);
    }
    .history-date-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.75rem 1rem;
      cursor: pointer;
      font-weight: var(--weight-semibold);
      font-size: var(--text-sm);
      color: var(--color-primary-dark);
      background: color-mix(in srgb, var(--color-primary) 6%, transparent);
      transition: background var(--duration-fast) var(--ease-out);
      &:hover {
        background: color-mix(in srgb, var(--color-primary) 12%, transparent);
      }
      .history-count {
        font-weight: var(--weight-normal);
        color: var(--color-text-muted);
        font-size: var(--text-xs);
        margin-inline-end: 0.5rem;
      }
    }
    .table { margin: 0; }
    .tab-nav {
      display: flex;
      border-bottom: 1px solid var(--color-border);
      background: var(--color-bg-subtle);
      margin-bottom: 1rem;
    }
    .tab-btn {
      padding: 0.75rem 1.25rem;
      border: none;
      background: none;
      font-size: 0.875rem;
      font-weight: 500;
      color: var(--color-text-secondary);
      cursor: pointer;
      transition: color var(--transition-fast);
      border-bottom: 2px solid transparent;
      margin-bottom: -1px;
      &:hover {
        color: var(--color-text);
      }
      &.active {
        color: var(--color-primary);
        border-bottom-color: var(--color-primary);
      }
    }
    .tab-content {
      padding: 0;
    }
    .search-dropdown {
      position: absolute;
      top: calc(100% + 4px);
      inset-inline-start: 0;
      right: 0;
      z-index: 1100;
      background: var(--color-bg-white);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-lg);
      max-height: 220px;
      overflow-y: auto;
    }
    .search-dropdown-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 0.5rem 0.75rem;
      cursor: pointer;
      border-bottom: 1px solid var(--color-border);
      transition: background var(--duration-fast);
      font-size: var(--text-sm);
      &:hover, &.highlighted {
        background: color-mix(in srgb, var(--color-primary) 8%, transparent);
      }
      &:last-child { border-bottom: none; }
      small { color: var(--color-text-muted); font-size: var(--text-xs); }
    }
    .selected-child-tag {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      margin-top: 0.35rem;
      padding: 0.35rem 0.6rem;
      background: color-mix(in srgb, var(--color-primary) 8%, transparent);
      border-radius: var(--radius-sm);
      font-size: var(--text-sm);
      font-weight: var(--weight-medium);
      .clear-btn {
        background: none;
        border: none;
        font-size: 1.1rem;
        line-height: 1;
        cursor: pointer;
        color: var(--color-text-muted);
        padding: 0 0.15rem;
        &:hover { color: var(--color-danger); }
      }
    }
  `],
})
export class AttendanceHistoryComponent {
  private readonly http = inject(HttpClient);
  private readonly attendanceService = inject(AttendanceService);
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly enrollmentService = inject(EnrollmentService);
  private readonly childService = inject(ChildService);
  private readonly translate = inject(TranslateService);
  private readonly brandingService = inject(BrandingService);
  private readonly toast = inject(ToastService);

  private templateHtml = '';
  private childReportTemplateHtml = '';
  private searchTimeout: ReturnType<typeof setTimeout> | null = null;

  readonly currentLang = this.translate.currentLang;
  loading = true;
  reportLoading = false;

  selectedYear = new Date().getFullYear();
  selectedMonth = 0;
  selectedPhaseId = 0;

  years: number[] = [];
  phases: KGPhase[] = [];

  private allAttendance: Attendance[] = [];
  private childPhaseMap = new Map<number, number>();

  readonly dateGroups = signal<Map<string, Attendance[]>>(new Map());
  readonly groupedDates = signal<string[]>([]);
  readonly expandedDates = signal<Set<string>>(new Set());

  readonly activeTab = signal<'register' | 'child-report'>('register');
  readonly childSearchTerm = signal('');
  readonly childSearchResults = signal<Child[]>([]);
  readonly selectedChild = signal<Child | null>(null);
  readonly childSearchOpen = signal(false);
  dateFrom = '';
  dateTo = '';

  constructor() {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    forkJoin({
      attendance: this.attendanceService.getAll(),
      phases: this.referenceDataService.getPhases(),
      enrollments: this.enrollmentService.getAll(),
    }).subscribe({
      next: ({ attendance, phases, enrollments }) => {
        this.allAttendance = attendance;
        this.phases = phases;

        const yearSet = new Set<number>();
        for (const r of attendance) {
          const y = new Date(r.date).getFullYear();
          if (!isNaN(y)) yearSet.add(y);
        }
        this.years = [...yearSet].sort((a, b) => b - a);
        if (this.years.length > 0 && !this.years.includes(this.selectedYear)) {
          this.selectedYear = this.years[0];
        }

        const latestPerChild = new Map<number, Enrollment>();
        for (const e of enrollments) {
          const existing = latestPerChild.get(e.childId);
          if (!existing || e.enrollmentDate > existing.enrollmentDate) {
            latestPerChild.set(e.childId, e);
          }
        }
        for (const [childId, enrollment] of latestPerChild) {
          this.childPhaseMap.set(childId, enrollment.kgPhaseId);
        }

        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toast.error(this.translate.instant('TOAST.LOAD_ERROR'));
      },
    });
  }

  applyFilters(): void {
    let filtered = this.allAttendance;

    filtered = filtered.filter(r => {
      const d = new Date(r.date);
      return !isNaN(d.getTime()) && d.getFullYear() === this.selectedYear;
    });
    if (this.selectedMonth > 0) {
      filtered = filtered.filter(r => {
        const d = new Date(r.date);
        return d.getMonth() + 1 === this.selectedMonth;
      });
    }
    if (this.selectedPhaseId) {
      filtered = filtered.filter(r => this.childPhaseMap.get(r.childId) === this.selectedPhaseId);
    }

    const map = new Map<string, Attendance[]>();
    for (const rec of filtered) {
      const key = rec.date.substring(0, 10);
      if (!map.has(key)) map.set(key, []);
      map.get(key)!.push(rec);
    }
    const sorted = [...map.keys()].sort((a, b) => b.localeCompare(a));
    this.dateGroups.set(map);
    this.groupedDates.set(sorted);
  }

  private async loadTemplate(): Promise<string> {
    if (this.templateHtml) return this.templateHtml;
    this.templateHtml = await firstValueFrom(
      this.http.get('assets/templates/attendance-register.html', { responseType: 'text' })
    );
    return this.templateHtml;
  }

  async printReport(): Promise<void> {
    if (this.selectedMonth === 0) {
      alert('الرجاء اختيار شهر محدد لتوليد التقرير'); return;
    }

    const template = await this.loadTemplate();
    const [{ default: jsPDF }, { default: html2canvas }] = await Promise.all([
      import('jspdf'),
      import('html2canvas'),
    ]);

    const year = this.selectedYear;
    const month = this.selectedMonth;
    const daysInMonth = new Date(year, month, 0).getDate();

    let records = this.allAttendance.filter(r => {
      const d = new Date(r.date);
      return d.getFullYear() === year && d.getMonth() + 1 === month;
    });
    if (this.selectedPhaseId) {
      records = records.filter(r => this.childPhaseMap.get(r.childId) === this.selectedPhaseId);
    }

    const childMap = new Map<number, { nameAr: string; nameEn: string; days: Map<number, string> }>();
    for (const r of records) {
      let child = childMap.get(r.childId);
      if (!child) {
        child = { nameAr: r.childNameAr, nameEn: r.childNameEn, days: new Map() };
        childMap.set(r.childId, child);
      }
      child.days.set(new Date(r.date).getDate(), r.status);
    }

    const children = [...childMap.values()];
    const phase = this.phases.find(p => p.id === this.selectedPhaseId);
    const phaseName = phase ? phase.nameAr : '---';

    const statusHtml = (s: string) => {
      switch (s) {
        case 'Present': return '<span class="present">✓</span>';
        case 'Absent': return '<span class="absent">A</span>';
        case 'Excused': return '<span class="excused">E</span>';
        default: return '';
      }
    };

    const dayHeaders: string[] = [];
    for (let d = 1; d <= daysInMonth; d++) dayHeaders.push(`<th class="day">${d}</th>`);

    const rowsHtml = children.map((c, i) => {
      const cells: string[] = [];
      for (let d = 1; d <= daysInMonth; d++) {
        const s = c.days.get(d);
        cells.push(`<td>${s ? statusHtml(s) : ''}</td>`);
      }
      return `<tr><td>${i + 1}</td><td class="name">${c.nameAr}</td>${cells.join('')}</tr>`;
    }).join('\n');

    const ARABIC_MONTHS = ['', 'يناير', 'فبراير', 'مارس', 'إبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];

    const html = template
      .replace('{{MONTH_NAME}}', ARABIC_MONTHS[month])
      .replace('{{YEAR}}', String(year))
      .replace('{{PHASE_NAME}}', phaseName)
      .replace('{{CHILD_COUNT}}', String(children.length))
      .replace('{{REPORT_DATE}}', new Date().toLocaleDateString('ar-EG'))
      .replace('{{DAY_HEADERS}}', dayHeaders.join(''))
      .replace('{{TABLE_ROWS}}', rowsHtml)
      .replace('{{APP_NAME_AR}}', this.brandingService.branding().appNameAr)
      .replace('{{APP_NAME_EN}}', this.brandingService.branding().appName);

    const outer = document.createElement('div');
    outer.style.cssText = 'position:fixed;top:-9999px;left:-9999px;';
    outer.innerHTML = html;
    document.body.appendChild(outer);
    const div = outer.firstElementChild as HTMLElement;

    try {
      await document.fonts.ready;
      const canvas = await html2canvas(div, {
        scale: 2, useCORS: true, backgroundColor: '#ffffff', logging: false,
      });
      const imgData = canvas.toDataURL('image/png');

      const doc = new jsPDF({ orientation: 'landscape', unit: 'px', format: 'a4' });
      const pageW = doc.internal.pageSize.getWidth();
      const pageH = doc.internal.pageSize.getHeight();
      const margin = 20;
      const maxH = pageH - margin * 2;
      const imgScale = Math.min((pageW - margin * 2) / canvas.width, maxH / canvas.height);
      const imgW = canvas.width * imgScale;
      const imgH = canvas.height * imgScale;
      const x = (pageW - imgW) / 2;

      if (imgH <= maxH) {
        doc.addImage(imgData, 'PNG', x, margin, imgW, imgH);
      } else {
        const sliceH = maxH;
        let srcY = 0;
        let page = 0;
        while (srcY < canvas.height) {
          if (page > 0) doc.addPage();
          const sliceCanvas = document.createElement('canvas');
          sliceCanvas.width = canvas.width;
          sliceCanvas.height = Math.min(sliceH / imgScale, canvas.height - srcY);
          const ctx = sliceCanvas.getContext('2d')!;
          ctx.drawImage(canvas, 0, srcY, canvas.width, sliceCanvas.height, 0, 0, canvas.width, sliceCanvas.height);
          const sliceData = sliceCanvas.toDataURL('image/png');
          doc.addImage(sliceData, 'PNG', x, margin, imgW, sliceCanvas.height * imgScale);
          srcY += sliceH / imgScale;
          page++;
        }
      }

      doc.save(`attendance-register-${year}-${String(month).padStart(2, '0')}.pdf`);
    } finally {
      document.body.removeChild(outer);
    }
  }

  onChildSearch(term: string): void {
    this.childSearchTerm.set(term);
    this.selectedChild.set(null);
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
    if (!term.trim()) { this.childSearchResults.set([]); return; }
    this.searchTimeout = setTimeout(() => {
      this.childService.getAll(term).subscribe({
        next: results => this.childSearchResults.set(results),
        error: () => this.childSearchResults.set([]),
      });
    }, 300);
  }

  selectChild(child: Child): void {
    this.selectedChild.set(child);
    this.childSearchTerm.set('');
    this.childSearchResults.set([]);
    this.childSearchOpen.set(false);
  }

  clearSelectedChild(): void {
    this.selectedChild.set(null);
    this.childSearchTerm.set('');
    this.childSearchResults.set([]);
  }

  onChildSearchBlur(): void {
    setTimeout(() => this.childSearchOpen.set(false), 200);
  }

  private async loadChildReportTemplate(): Promise<string> {
    if (this.childReportTemplateHtml) return this.childReportTemplateHtml;
    this.childReportTemplateHtml = await firstValueFrom(
      this.http.get('assets/templates/child-attendance-report.html', { responseType: 'text' })
    );
    return this.childReportTemplateHtml;
  }

  async generateChildReport(): Promise<void> {
    const child = this.selectedChild();
    if (!child) { alert(this.translate.instant('ATTENDANCE.SELECT_CHILD_FIRST')); return; }
    if (!this.dateFrom || !this.dateTo) { alert(this.translate.instant('ATTENDANCE.SELECT_DATE_RANGE')); return; }

    this.reportLoading = true;
    try {
      const template = await this.loadChildReportTemplate();
      const [{ default: jsPDF }, { default: html2canvas }] = await Promise.all([
        import('jspdf'),
        import('html2canvas'),
      ]);

      const from = new Date(this.dateFrom);
      const to = new Date(this.dateTo);
      to.setHours(23, 59, 59, 999);

      const allRecords = await firstValueFrom(this.attendanceService.getAll());
      const records = allRecords.filter(r => {
        if (r.childId !== child.id) return false;
        const d = new Date(r.date);
        return d >= from && d <= to;
      });
      records.sort((a, b) => a.date.localeCompare(b.date));

      let present = 0, absent = 0, excused = 0;
      const statusColor: Record<string, string> = { Present: 'green', Absent: 'red', Excused: '#1565c0' };
      const statusLabel: Record<string, string> = {
        Present: 'حاضر', Absent: 'غائب', Excused: 'معذر',
      };
      const dayNames = ['الأحد', 'الإثنين', 'الثلاثاء', 'الأربعاء', 'الخميس', 'الجمعة', 'السبت'];

      for (const r of records) {
        if (r.status === 'Present') present++;
        else if (r.status === 'Absent') absent++;
        else if (r.status === 'Excused') excused++;
      }
      const total = records.length;
      const pct = total > 0 ? Math.round((present / total) * 100) : 0;

      const tableRows = records.map(r => {
        const d = new Date(r.date);
        const dayName = dayNames[d.getDay()];
        const dateStr = d.toLocaleDateString('ar-EG');
        const color = statusColor[r.status] || 'inherit';
        const label = statusLabel[r.status] || r.status;
        return `<tr><td>${dateStr}</td><td>${dayName}</td><td style="color:${color}">${label}</td><td>${r.notes || ''}</td></tr>`;
      }).join('\n');

      const enrollment = await firstValueFrom(this.enrollmentService.getByChild(child.id));
      const latestEnrollment = enrollment.length > 0 ? enrollment.reduce((a, b) =>
        a.enrollmentDate > b.enrollmentDate ? a : b
      ) : null;
      const phaseName = latestEnrollment
        ? (this.currentLang() === 'ar' ? latestEnrollment.kgPhaseNameAr : latestEnrollment.kgPhaseNameEn)
        : '---';

      const html = template
        .replace('{{APP_NAME_AR}}', this.brandingService.branding().appNameAr)
        .replace('{{APP_NAME_EN}}', this.brandingService.branding().appName)
        .replace('{{CHILD_NAME}}', this.currentLang() === 'ar' ? child.fullNameAr : child.fullNameEn)
        .replace('{{PHASE_NAME}}', phaseName)
        .replace('{{DATE_FROM}}', from.toLocaleDateString('ar-EG'))
        .replace('{{DATE_TO}}', to.toLocaleDateString('ar-EG'))
        .replace('{{PRESENT_COUNT}}', String(present))
        .replace('{{ABSENT_COUNT}}', String(absent))
        .replace('{{EXCUSED_COUNT}}', String(excused))
        .replace('{{ATTENDANCE_PERCENTAGE}}', String(pct))
        .replace('{{TABLE_ROWS}}', tableRows)
        .replace('{{NOTES}}', '');

      const outer = document.createElement('div');
      outer.style.cssText = 'position:fixed;top:-9999px;left:-9999px;';
      outer.innerHTML = html;
      document.body.appendChild(outer);
      const div = outer.firstElementChild as HTMLElement;

      try {
        await document.fonts.ready;
        const canvas = await html2canvas(div, {
          scale: 2, useCORS: true, backgroundColor: '#ffffff', logging: false,
        });
        const imgData = canvas.toDataURL('image/png');

        const doc = new jsPDF({ orientation: 'portrait', unit: 'px', format: 'a4' });
        const pageW = doc.internal.pageSize.getWidth();
        const pageH = doc.internal.pageSize.getHeight();
        const margin = 20;
        const maxH = pageH - margin * 2;
        const imgScale = Math.min((pageW - margin * 2) / canvas.width, maxH / canvas.height);
        const imgW = canvas.width * imgScale;
        const imgH = canvas.height * imgScale;
        const x = (pageW - imgW) / 2;

        if (imgH <= maxH) {
          doc.addImage(imgData, 'PNG', x, margin, imgW, imgH);
        } else {
          const sliceH = maxH;
          let srcY = 0;
          let page = 0;
          while (srcY < canvas.height) {
            if (page > 0) doc.addPage();
            const sliceCanvas = document.createElement('canvas');
            sliceCanvas.width = canvas.width;
            sliceCanvas.height = Math.min(sliceH / imgScale, canvas.height - srcY);
            const ctx = sliceCanvas.getContext('2d')!;
            ctx.drawImage(canvas, 0, srcY, canvas.width, sliceCanvas.height, 0, 0, canvas.width, sliceCanvas.height);
            const sliceData = sliceCanvas.toDataURL('image/png');
            doc.addImage(sliceData, 'PNG', x, margin, imgW, sliceCanvas.height * imgScale);
            srcY += sliceH / imgScale;
            page++;
          }
        }

        const fileName = `child-attendance-${child.id}-${this.dateFrom}-${this.dateTo}.pdf`;
        doc.save(fileName);
      } finally {
        document.body.removeChild(outer);
      }
    } finally {
      this.reportLoading = false;
    }
  }

  toggleDate(dateKey: string): void {
    this.expandedDates.update(s => {
      const next = new Set(s);
      if (next.has(dateKey)) next.delete(dateKey); else next.add(dateKey);
      return next;
    });
  }
}
