export type StrategyType = "Snowball" | "Avalanche";

export interface RegisterDto {
  email: string;
  fullName: string;
  password: string;
  monthlyRepaymentBudget: number;
  repaymentStrategy: string;
}