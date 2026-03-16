const BASE = "http://localhost:5099";

function authHeaders() {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  };
}

export interface UserProfile {
  userId: string;
  email: string;
  fullName: string;
  totalDebt: number;
  monthlyRepaymentBudget: number;
  repaymentStrategy: number; // 1 = Snowball, 2 = Avalanche
}

export async function getMe(): Promise<UserProfile | null> {
  try {
    const response = await fetch(`${BASE}/api/v1/Auth/me`, {
      headers: authHeaders(),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to load profile", error);
    return null;
  }
}
