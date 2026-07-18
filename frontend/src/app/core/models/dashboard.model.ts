export interface DashboardSummary {
  totalStudents: number;
  activeStudents: number;
  dailyAbsentCount: number;
  absentRate: number;
  dailyIncome: number;
  monthlyIncome: number;
  pendingPaymentsCount: number;
  pendingPaymentsAmount: number;
  phaseDistribution: PhaseDistributionItem[];
  paymentStatusDistribution: PaymentStatusItem[];
  recentPayments: RecentPaymentItem[];
  enrollmentTrends: MonthlyEnrollmentTrend[];
}

export interface PhaseDistributionItem {
  phaseNameAr: string;
  phaseNameEn: string;
  studentCount: number;
}

export interface PaymentStatusItem {
  status: string;
  count: number;
  amount: number;
}

export interface RecentPaymentItem {
  id: number;
  childNameAr: string;
  childNameEn: string;
  amount: number;
  status: string;
  paidDate?: string;
}

export interface MonthlyEnrollmentTrend {
  month: string;
  count: number;
}
