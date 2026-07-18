export interface Payment {
  id: number;
  enrollmentId: number;
  childNameAr: string;
  childNameEn: string;
  kgPhaseId: number;
  kgPhaseNameAr: string;
  kgPhaseNameEn: string;
  month: number;
  year: number;
  amountDue: number;
  amountPaid: number;
  discount: number;
  remaining: number;
  dueDate: string;
  paidDate?: string;
  status: string;
  method: string;
  notes?: string;
  receivedBy?: string;
}
