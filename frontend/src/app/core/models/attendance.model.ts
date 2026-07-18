export interface Attendance {
  id: number;
  childId: number;
  childNameAr: string;
  childNameEn: string;
  date: string;
  status: string;
  notes?: string;
}
