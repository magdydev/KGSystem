import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DateInputComponent } from '../../../shared/components/date-input/date-input.component';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Attendance } from '../../../core/models/attendance.model';
import { Child } from '../../../core/models/child.model';
import { AttendanceService } from '../../../core/services/attendance.service';
import { ChildService } from '../../../core/services/child.service';
import { ReferenceDataService } from '../../../core/services/reference-data.service';
import { ToastService } from '../../../core/services/toast.service';

interface ChildAttendance {
  childId: number;
  childNameAr: string;
  childNameEn: string;
  status: string;
  notes: string;
}

interface PhaseGroup {
  id: number;
  nameAr: string;
  nameEn: string;
  children: ChildAttendance[];
}

@Component({
  selector: 'app-attendance-list',
  standalone: true,
  imports: [FormsModule, TranslatePipe, DateInputComponent],
  templateUrl: './attendance-list.component.html',
  styleUrl: './attendance-list.component.scss',
})
export class AttendanceListComponent implements OnInit {
  private readonly attendanceService = inject(AttendanceService);
  private readonly childService = inject(ChildService);
  private readonly referenceDataService = inject(ReferenceDataService);
  private readonly toast = inject(ToastService);
  private readonly translate = inject(TranslateService);

  readonly currentLang = this.translate.currentLang;

  readonly statuses = ['Present', 'Absent', 'Excused', 'Late'];

  date = new Date().toISOString().substring(0, 10);
  saving = false;
  loading = true;

  readonly phaseGroups = signal<PhaseGroup[]>([]);
  readonly selectedPhaseId = signal<number>(0);

  readonly phases = computed(() =>
    this.phaseGroups().map(g => ({ id: g.id, nameAr: g.nameAr, nameEn: g.nameEn }))
  );

  readonly currentChildren = computed(() => {
    const id = this.selectedPhaseId();
    return this.phaseGroups().find(g => g.id === id)?.children ?? [];
  });

  readonly   selectedPhaseName = computed(() => {
    const id = this.selectedPhaseId();
    const phase = this.phases().find(p => p.id === id);
    return phase ? (this.currentLang() === 'ar' ? phase.nameAr : phase.nameEn) : '';
  });

  readonly expandedDates = signal<Set<string>>(new Set());

  markAllPresent(): void {
    this.phaseGroups.update(groups =>
      groups.map(g => ({
        ...g,
        children: g.children.map(r =>
          r.status !== 'Present'
            ? { ...r, status: 'Present' }
            : r
        ),
      }))
    );
  }

  toggleDateGroup(dateKey: string): void {
    this.expandedDates.update(s => {
      const next = new Set(s);
      if (next.has(dateKey)) next.delete(dateKey); else next.add(dateKey);
      return next;
    });
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;

    forkJoin({
      phases: this.referenceDataService.getPhases(),
      attendance: this.attendanceService.getAll(this.date),
    }).subscribe({
      next: ({ phases, attendance }) => {
        const attMap = new Map<number, Attendance>();
        for (const a of attendance) {
          attMap.set(a.childId, a);
        }

        const obs = phases.map(phase =>
          this.childService.getAll(undefined, 'ACTIVE', phase.id).pipe(
            map(children => ({ phase, children })),
            catchError(() => of({ phase, children: [] as Child[] }))
          )
        );

        forkJoin(obs).subscribe(results => {
          const groups: PhaseGroup[] = [];
          for (const { phase, children } of results) {
            groups.push({
              id: phase.id,
              nameAr: phase.nameAr,
              nameEn: phase.nameEn,
              children: children.map(child => {
                const existing = attMap.get(child.id);
                return {
                  childId: child.id,
                  childNameAr: child.fullNameAr,
                  childNameEn: child.fullNameEn,
                  status: existing?.status ?? 'Present',
                  notes: existing?.notes ?? '',
                };
              }),
            });
          }
          this.phaseGroups.set(groups);
          if (groups.length > 0 && !this.selectedPhaseId()) {
            this.selectedPhaseId.set(groups[0].id);
          }
          this.loading = false;
        });
      },
      error: () => {
        this.loading = false;
        this.toast.error(this.translate.instant('TOAST.LOAD_ERROR'));
      },
    });
  }

  onDateChange(): void {
    this.loadData();
  }

  save(): void {
    if (this.saving) return;
    this.saving = true;

    const records = this.currentChildren();

    this.attendanceService.createBatch({
      date: this.date,
      records: records.map(r => ({
        childId: r.childId,
        status: r.status,
        notes: r.notes || undefined,
      })),
    }).subscribe({
      next: () => {
        this.saving = false;
        this.loadData();
        this.toast.success(this.translate.instant('TOAST.ATTENDANCE_SAVED'));
      },
      error: () => {
        this.saving = false;
        this.toast.error(this.translate.instant('TOAST.ATTENDANCE_SAVE_ERROR'));
      },
    });
  }
}
