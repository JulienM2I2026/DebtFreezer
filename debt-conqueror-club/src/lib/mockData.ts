// Mock data for the entire application

export interface Debt {
  id: string;
  creditor: string;
  originalAmount: number;
  remainingAmount: number;
  interestRate: number;
  dueDate: string;
  type: 'credit-card' | 'personal-loan' | 'student-loan';
  minPayment: number;
}

export interface Payment {
  id: string;
  debtId: string;
  amount: number;
  date: string;
  notes: string;
}

export interface Challenge {
  id: string;
  name: string;
  goal: number;
  currentAmount: number;
  startDate: string;
  endDate: string;
  members: ChallengeMember[];
}

export interface ChallengeMember {
  id: string;
  name: string;
  avatar: string;
  contributed: number;
  streak: number;
}

export const mockDebts: Debt[] = [
  { id: '1', creditor: 'Chase Sapphire', originalAmount: 8500, remainingAmount: 5200, interestRate: 19.99, dueDate: '2025-06-15', type: 'credit-card', minPayment: 150 },
  { id: '2', creditor: 'Student Loan Fed', originalAmount: 32000, remainingAmount: 24800, interestRate: 5.5, dueDate: '2030-01-01', type: 'student-loan', minPayment: 350 },
  { id: '3', creditor: 'Personal Loan - SoFi', originalAmount: 12000, remainingAmount: 7600, interestRate: 8.99, dueDate: '2027-03-20', type: 'personal-loan', minPayment: 280 },
  { id: '4', creditor: 'Amex Gold', originalAmount: 4200, remainingAmount: 1800, interestRate: 22.49, dueDate: '2025-04-10', type: 'credit-card', minPayment: 85 },
  { id: '5', creditor: 'Car Loan - Capital One', originalAmount: 18000, remainingAmount: 11200, interestRate: 6.5, dueDate: '2028-08-01', type: 'personal-loan', minPayment: 420 },
];

export const mockPayments: Payment[] = [
  { id: '1', debtId: '1', amount: 300, date: '2025-02-01', notes: 'Extra payment this month' },
  { id: '2', debtId: '2', amount: 350, date: '2025-02-01', notes: 'Regular payment' },
  { id: '3', debtId: '3', amount: 500, date: '2025-02-15', notes: 'Bonus payment' },
  { id: '4', debtId: '4', amount: 200, date: '2025-02-10', notes: 'Trying to pay this off fast' },
  { id: '5', debtId: '1', amount: 150, date: '2025-01-15', notes: 'Minimum payment' },
  { id: '6', debtId: '5', amount: 420, date: '2025-02-01', notes: 'Regular car payment' },
  { id: '7', debtId: '2', amount: 350, date: '2025-01-01', notes: 'Regular payment' },
  { id: '8', debtId: '3', amount: 280, date: '2025-01-15', notes: 'Regular payment' },
];

export const mockChallenges: Challenge[] = [
  {
    id: '1',
    name: 'Debt Crushers 2025',
    goal: 10000,
    currentAmount: 6340,
    startDate: '2025-01-01',
    endDate: '2025-06-30',
    members: [
      { id: '1', name: 'You', avatar: 'Y', contributed: 2100, streak: 14 },
      { id: '2', name: 'Sarah M.', avatar: 'S', contributed: 1850, streak: 12 },
      { id: '3', name: 'Alex K.', avatar: 'A', contributed: 1390, streak: 9 },
      { id: '4', name: 'Jordan P.', avatar: 'J', contributed: 1000, streak: 7 },
    ],
  },
  {
    id: '2',
    name: 'Credit Card Freedom',
    goal: 5000,
    currentAmount: 2200,
    startDate: '2025-02-01',
    endDate: '2025-08-01',
    members: [
      { id: '1', name: 'You', avatar: 'Y', contributed: 800, streak: 5 },
      { id: '5', name: 'Taylor R.', avatar: 'T', contributed: 700, streak: 4 },
      { id: '6', name: 'Morgan L.', avatar: 'M', contributed: 700, streak: 6 },
    ],
  },
];

export const debtEvolutionData = [
  { month: 'Sep', total: 58000 },
  { month: 'Oct', total: 56200 },
  { month: 'Nov', total: 54800 },
  { month: 'Dec', total: 53100 },
  { month: 'Jan', total: 52000 },
  { month: 'Feb', total: 50600 },
];

export const paymentHistoryData = [
  { month: 'Sep', paid: 1800 },
  { month: 'Oct', paid: 2100 },
  { month: 'Nov', paid: 1900 },
  { month: 'Dec', paid: 2300 },
  { month: 'Jan', paid: 2550 },
  { month: 'Feb', paid: 2470 },
];

export const debtByTypeData = [
  { name: 'Credit Cards', value: 7000, fill: 'hsl(239, 84%, 67%)' },
  { name: 'Student Loans', value: 24800, fill: 'hsl(217, 91%, 60%)' },
  { name: 'Personal Loans', value: 18800, fill: 'hsl(142, 71%, 45%)' },
];
