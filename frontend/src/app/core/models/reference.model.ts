export interface KGPhase {
  id: string;
  code: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  sortOrder: number;
}

export interface AcademicYear {
  id: string;
  code: string;
  nameAr: string;
  nameEn: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
}

export interface MonthlyFee {
  id: string;
  academicYearId: string;
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
  academicYearId: string;
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
