export interface Enrollment {
  id: string;
  childId: string;
  childNameAr: string;
  childNameEn: string;
  kgPhaseId: string;
  kgPhaseNameAr: string;
  kgPhaseNameEn: string;
  academicYearId: string;
  academicYearNameAr: string;
  academicYearNameEn: string;
  enrollmentDate: string;
  status: string;
  notes?: string;
}
