import { Enrollment } from './enrollment.model';
import { Payment } from './payment.model';

export interface KGPhase {
  id: number;
  code: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  sortOrder: number;
}

export interface AcademicYear {
  id: number;
  code: string;
  nameAr: string;
  nameEn: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface MonthlyFee {
  id: number;
  academicYearId: number;
  academicYearNameAr: string;
  academicYearNameEn: string;
  month: number;
  amount: number;
  currency: string;
  dueDate: string | null;
}

export interface MonthlyFeeItem {
  month: number;
  amount: number;
  dueDate: string | null;
}

export interface PatchMonthlyFeesRequest {
  academicYearId: number;
  fees: MonthlyFeeItem[];
}

export interface CreateKGPhaseRequest {
  code: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  sortOrder: number;
}

export interface CreateAcademicYearRequest {
  code: string;
  nameAr: string;
  nameEn: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface ArchivedYearDetail {
  year: AcademicYear;
  enrollments: Enrollment[];
  payments: Payment[];
}
