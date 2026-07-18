export interface Payment {
  id: string;
  enrollmentId: string;
  childNameAr: string;
  childNameEn: string;
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
