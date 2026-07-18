import { Enrollment } from './enrollment.model';

export interface Child {
  id: string;
  firstNameAr: string;
  firstNameEn: string;
  lastNameAr: string;
  lastNameEn: string;
  fullNameAr: string;
  fullNameEn: string;
  dateOfBirth: string;
  gender: string;
  nationality?: string;
  guardianNameAr: string;
  guardianNameEn: string;
  guardianPhone: string;
  guardianEmail?: string;
  address?: string;
  photoUrl?: string;
  status: string;
  enrollmentDate: string;
  createdAt: string;
}

export interface ChildDetail extends Child {
  enrollments: Enrollment[];
}

export interface CreateChildRequest {
  firstNameAr: string;
  firstNameEn: string;
  lastNameAr: string;
  lastNameEn: string;
  dateOfBirth: string;
  gender: string;
  guardianNameAr: string;
  guardianNameEn: string;
  guardianPhone: string;
  guardianEmail?: string;
  nationality?: string;
  address?: string;
  kgPhaseId?: string;
  academicYearId?: string;
}

