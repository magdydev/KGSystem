import { NgClass } from '@angular/common';
import { Component, effect, ElementRef, forwardRef, HostListener, input, OnDestroy, signal, viewChild } from '@angular/core';
import { NG_VALUE_ACCESSOR, ControlValueAccessor, FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

interface DayCell {
  day: number;
  month: number;
  year: number;
  isCurrentMonth: boolean;
  isToday: boolean;
}

const MONTH_NAMES_EN = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
const MONTH_NAMES_AR = ['يناير', 'فبراير', 'مارس', 'إبريل', 'مايو', 'يونيو', 'يوليو', 'أغسطس', 'سبتمبر', 'أكتوبر', 'نوفمبر', 'ديسمبر'];
const WEEKDAYS_EN = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const WEEKDAYS_AR = ['أحد', 'اثن', 'ثلاث', 'أربع', 'خميس', 'جمعة', 'سبت'];

@Component({
  selector: 'app-date-input',
  standalone: true,
  imports: [FormsModule, NgClass, TranslatePipe],
  template: `
    <div class="date-wrapper" #wrapperRef>
      <input
        type="text"
        class="date-input"
        [value]="displayValue"
        [placeholder]="'COMMON.SELECT_DATE' | translate"
        [disabled]="disabled()"
        [attr.name]="name() || null"
        (focus)="open()"
        (input)="onInput($event)"
        readonly
      />
      <svg class="date-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" (click)="toggle()">
        <rect x="3" y="4" width="18" height="18" rx="2" ry="2"/>
        <line x1="16" y1="2" x2="16" y2="6"/>
        <line x1="8" y1="2" x2="8" y2="6"/>
        <line x1="3" y1="10" x2="21" y2="10"/>
      </svg>

      @if (isOpen()) {
        <div class="cal-popup" (click)="$event.stopPropagation()" [class.rtl]="isRtl">
          <div class="cal-header">
            <button type="button" class="cal-nav" (click)="prevMonth()">‹</button>
            <div class="cal-title" (click)="toggleYearPicker()">
              {{ monthNames[currentMonth] }} {{ currentYear }}
              @if (showYearSelector()) {
                <div class="cal-year-dropdown" (click)="$event.stopPropagation()">
                  @for (y of yearRange; track y) {
                    <button type="button" class="cal-year-opt" [class.active]="y === currentYear" (click)="selectYear(y)">{{ y }}</button>
                  }
                </div>
              }
            </div>
            <button type="button" class="cal-nav" (click)="nextMonth()">›</button>
          </div>

          <div class="cal-weekdays">
            @for (wd of weekdays; track $index) {
              <span class="cal-weekday">{{ wd }}</span>
            }
          </div>

          <div class="cal-grid">
            @for (cell of days; track $index) {
              @if (cell) {
                <button
                  type="button"
                  class="cal-day"
                  [class.current-month]="cell.isCurrentMonth"
                  [class.today]="cell.isToday"
                  [class.selected]="isSelected(cell)"
                  (click)="selectDate(cell)"
                >{{ cell.day }}</button>
              } @else {
                <span class="cal-day empty"></span>
              }
            }
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .date-wrapper {
      position: relative;
      display: flex;
      align-items: center;
      width: 100%;
    }

    .date-input {
      width: 100%;
      padding: 0.6rem 0.85rem 0.6rem 2.4rem;
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      font-size: var(--text-sm);
      font-family: var(--font-body);
      color: var(--color-text);
      background: var(--color-bg-white);
      outline: none;
      transition: border-color var(--duration-fast) var(--ease-out),
                  box-shadow var(--duration-fast) var(--ease-out);
      box-sizing: border-box;
      line-height: 1.5;
      min-height: 42px;
      font-weight: var(--weight-normal);
      cursor: pointer;

      &:hover {
        border-color: var(--color-text-muted);
      }

      &:focus {
        border-color: var(--color-primary);
        box-shadow: 0 0 0 2px color-mix(in srgb, var(--color-primary) 20%, transparent);
      }

      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
        background: var(--color-bg-muted);
      }

      &::placeholder {
        color: var(--color-text-muted);
        font-weight: var(--weight-normal);
      }
    }

    .date-icon {
      position: absolute;
      inset-inline-start: 10px;
      width: 18px;
      height: 18px;
      color: var(--color-text-muted);
      opacity: 0.5;
      pointer-events: all;
      cursor: pointer;
      transition: opacity var(--transition-fast), scale var(--transition-fast);

      &:hover {
        opacity: 0.8;
        scale: 1.1;
      }
    }

    .date-wrapper:focus-within .date-icon {
      opacity: 0.8;
    }

    :host-context(html[dir='rtl']) .date-input {
      padding: 0.6rem 2.4rem 0.6rem 0.85rem;
    }

    :host-context(html[dir='rtl']) .date-icon {
      inset-inline-start: auto;
      inset-inline-end: 10px;
    }

    .cal-popup {
      position: absolute;
      top: calc(100% + 6px);
      inset-inline-start: 0;
      z-index: 1100;
      background: linear-gradient(145deg, color-mix(in srgb, var(--color-primary) 12%, var(--color-bg-white)), color-mix(in srgb, var(--color-secondary) 6%, var(--color-bg-white)));
      border: 1.5px solid color-mix(in srgb, var(--color-primary) 30%, var(--color-border));
      border-radius: var(--radius-lg);
      box-shadow: var(--shadow-xl), 0 0 0 1px color-mix(in srgb, var(--color-primary) 8%, transparent) inset;
      padding: 0.75rem;
      width: 282px;
      font-family: var(--font-body);
      animation: cal-fade-in 0.15s var(--ease-out);

      &.rtl {
        direction: rtl;
      }
    }

    .cal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 0.75rem;
      gap: 0.5rem;
      background: color-mix(in srgb, var(--color-primary) 10%, var(--color-primary-light));
      margin: -0.75rem -0.75rem 0.75rem -0.75rem;
      padding: 0.6rem 0.5rem;
      border-radius: var(--radius-lg) var(--radius-lg) 0 0;
      border-bottom: 1px solid color-mix(in srgb, var(--color-primary) 15%, transparent);
    }

    .cal-nav {
      background: none;
      border: none;
      font-size: 1.3rem;
      line-height: 1;
      color: var(--color-primary);
      cursor: pointer;
      padding: 0.25rem 0.55rem;
      border-radius: var(--radius-sm);
      transition: all var(--transition-fast);
      font-weight: var(--weight-bold);

      &:hover {
        background: color-mix(in srgb, var(--color-primary) 15%, transparent);
        color: var(--color-primary-dark);
        scale: 1.15;
      }
    }

    .cal-title {
      font-size: 0.9375rem;
      font-weight: var(--weight-bold);
      color: var(--color-primary-dark);
      cursor: pointer;
      padding: 0.25rem 0.6rem;
      border-radius: var(--radius-sm);
      position: relative;
      user-select: none;
      letter-spacing: -0.01em;

      &:hover {
        background: color-mix(in srgb, var(--color-primary) 12%, transparent);
      }
    }

    .cal-year-dropdown {
      position: absolute;
      top: 100%;
      inset-inline-start: 50%;
      transform: translateX(-50%);
      background: var(--color-bg-white);
      border: 1px solid var(--color-border);
      border-radius: var(--radius-md);
      box-shadow: var(--shadow-lg);
      max-height: 180px;
      overflow-y: auto;
      z-index: 1110;
      width: 100px;
      padding: 0.25rem;
    }

    :host-context(html[dir='rtl']) .cal-year-dropdown {
      transform: translateX(50%);
    }

    .cal-year-opt {
      display: block;
      width: 100%;
      padding: 0.35rem 0.5rem;
      border: none;
      border-radius: var(--radius-sm);
      background: none;
      font-size: 0.8125rem;
      font-family: inherit;
      color: var(--color-text);
      cursor: pointer;
      text-align: center;
      transition: all var(--transition-fast);

      &:hover {
        background: var(--color-primary-light);
        color: var(--color-primary-dark);
      }

      &.active {
        background: var(--color-primary);
        color: var(--color-primary-contrast);
        font-weight: var(--weight-semibold);
      }
    }

    .cal-weekdays {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      margin-bottom: 0.15rem;
      padding: 0 0.15rem;
    }

    .cal-weekday {
      text-align: center;
      font-size: 0.7rem;
      font-weight: var(--weight-bold);
      color: var(--color-primary);
      opacity: 0.6;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      padding: 0.3rem 0;
    }

    .cal-grid {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      gap: 2px;
      padding: 0 0.15rem;
    }

    .cal-day {
      aspect-ratio: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      border: none;
      border-radius: var(--radius-md);
      font-size: 0.8125rem;
      font-weight: var(--weight-medium);
      color: var(--color-text-muted);
      background: transparent;
      cursor: default;
      transition: all var(--duration-fast) var(--ease-out);
      padding: 0;

      &.current-month {
        color: var(--color-text);
        cursor: pointer;
        background: color-mix(in srgb, var(--color-primary) 4%, transparent);

        &:hover {
          background: color-mix(in srgb, var(--color-primary) 18%, var(--color-primary-light));
          color: var(--color-primary-dark);
          scale: 1.05;
        }
      }

      &.today {
        border: 1.5px solid var(--color-primary);
        font-weight: var(--weight-bold);
        color: var(--color-primary-dark);
        background: color-mix(in srgb, var(--color-primary) 8%, transparent);
      }

      &.selected {
        background: linear-gradient(135deg, var(--color-primary), var(--color-primary-dark));
        color: var(--color-primary-contrast);
        font-weight: var(--weight-bold);
        box-shadow: 0 3px 10px color-mix(in srgb, var(--color-primary) 35%, transparent);

        &:hover {
          scale: 1.08;
          background: linear-gradient(135deg, var(--color-primary-dark), var(--color-primary));
        }

        &.today {
          border-color: var(--color-primary-contrast);
        }
      }

      &.empty {
        cursor: default;
      }
    }

    @keyframes cal-fade-in {
      from {
        opacity: 0;
        transform: translateY(-4px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
  `],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateInputComponent),
      multi: true,
    },
  ],
})
export class DateInputComponent implements ControlValueAccessor, OnDestroy {
  readonly name = input<string>();
  readonly required = input(false);
  readonly min = input<string>();
  readonly max = input<string>();
  readonly disabled = input(false);

  readonly wrapperRef = viewChild.required<ElementRef<HTMLElement>>('wrapperRef');

  value = '';
  isOpen = signal(false);
  showYearSelector = signal(false);
  currentMonth = new Date().getMonth();
  currentYear = new Date().getFullYear();
  displayValue = '';

  monthNames = MONTH_NAMES_EN;
  weekdays = WEEKDAYS_EN;
  isRtl = false;

  get yearRange(): number[] {
    const years: number[] = [];
    for (let y = this.currentYear - 10; y <= this.currentYear + 10; y++) {
      years.push(y);
    }
    return years;
  }

  get days(): (DayCell | null)[] {
    const firstDay = new Date(this.currentYear, this.currentMonth, 1).getDay();
    const totalDays = new Date(this.currentYear, this.currentMonth + 1, 0).getDate();
    const today = new Date();

    const cells: (DayCell | null)[] = [];
    for (let i = 0; i < firstDay; i++) {
      cells.push(null);
    }
    for (let d = 1; d <= totalDays; d++) {
      cells.push({
        day: d,
        month: this.currentMonth,
        year: this.currentYear,
        isCurrentMonth: true,
        isToday: d === today.getDate() && this.currentMonth === today.getMonth() && this.currentYear === today.getFullYear(),
      });
    }
    const remainder = cells.length % 7;
    if (remainder > 0) {
      for (let i = 0; i < 7 - remainder; i++) {
        cells.push(null);
      }
    }
    return cells;
  }

  protected onChange: (val: string) => void = () => {};
  protected onTouched: () => void = () => {};

  constructor() {
    this.isRtl = document.documentElement.dir === 'rtl';
    if (this.isRtl) {
      this.monthNames = MONTH_NAMES_AR;
      this.weekdays = WEEKDAYS_AR;
    }
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const wrapper = this.wrapperRef()?.nativeElement;
    if (wrapper && !wrapper.contains(event.target as Node)) {
      this.close();
    }
  }

  open(): void {
    if (this.disabled()) return;
    if (this.value) {
      const d = new Date(this.value);
      if (!isNaN(d.getTime())) {
        this.currentMonth = d.getMonth();
        this.currentYear = d.getFullYear();
      }
    }
    this.isOpen.set(true);
  }

  close(): void {
    this.isOpen.set(false);
    this.showYearSelector.set(false);
    this.onTouched();
  }

  toggle(): void {
    if (this.isOpen()) {
      this.close();
    } else {
      this.open();
    }
  }

  toggleYearPicker(): void {
    this.showYearSelector.update(v => !v);
  }

  selectYear(y: number): void {
    this.currentYear = y;
    this.showYearSelector.set(false);
  }

  prevMonth(): void {
    if (this.currentMonth === 0) {
      this.currentMonth = 11;
      this.currentYear--;
    } else {
      this.currentMonth--;
    }
  }

  nextMonth(): void {
    if (this.currentMonth === 11) {
      this.currentMonth = 0;
      this.currentYear++;
    } else {
      this.currentMonth++;
    }
  }

  isSelected(cell: DayCell): boolean {
    if (!this.value) return false;
    const d = new Date(this.value);
    return d.getDate() === cell.day && d.getMonth() === cell.month && d.getFullYear() === cell.year;
  }

  selectDate(cell: DayCell): void {
    const month = String(cell.month + 1).padStart(2, '0');
    const day = String(cell.day).padStart(2, '0');
    const dateStr = `${cell.year}-${month}-${day}`;
    this.value = dateStr;
    this.displayValue = this.formatDisplay(cell);
    this.onChange(dateStr);
    this.close();
  }

  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.displayValue = input.value;
  }

  private formatDisplay(cell: DayCell): string {
    const m = this.monthNames[cell.month];
    return `${m} ${cell.day}, ${cell.year}`;
  }

  private updateDisplay(): void {
    if (this.value) {
      const d = new Date(this.value);
      if (!isNaN(d.getTime())) {
        this.displayValue = `${this.monthNames[d.getMonth()]} ${d.getDate()}, ${d.getFullYear()}`;
        return;
      }
    }
    this.displayValue = '';
  }

  writeValue(val: string): void {
    if (!val) {
      const today = new Date();
      const m = String(today.getMonth() + 1).padStart(2, '0');
      const d = String(today.getDate()).padStart(2, '0');
      this.value = `${today.getFullYear()}-${m}-${d}`;
      this.currentMonth = today.getMonth();
      this.currentYear = today.getFullYear();
      this.onChange(this.value);
    } else {
      this.value = val;
    }
    this.updateDisplay();
  }

  registerOnChange(fn: (val: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  ngOnDestroy(): void {
    // cleanup handled by Angular
  }
}
