export type DebtType = 0 | 1 | 2; // correspond à CREDIT_CARD = 0, PERSONAL_LOAN = 1, STUDENT_LOAN = 2
export type DebtStatus = 0 | 1;  // ACTIVE = 0, PAID_OFF = 1

export interface DebtCreateUpdateDto {
  id?: number
  creditor: string
  originalAmount: number
  interestRate: number
  dueDate: string
  type: number
  status?: number
}