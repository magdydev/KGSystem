export interface Enrollment {
  id: number;
  childId: number;
  childNameAr: string;
  childNameEn: string;
  kgPhaseId: number;
  kgPhaseNameAr: string;
  kgPhaseNameEn: string;
  academicYearId: number;
  academicYearNameAr: string;
  academicYearNameEn: string;
  enrollmentDate: string;
  status: string;
  notes?: string;
}
