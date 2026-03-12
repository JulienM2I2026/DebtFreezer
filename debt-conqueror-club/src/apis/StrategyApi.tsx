const BASE = "http://localhost:5099";

function authHeaders() {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  };
}

export interface RepaymentPlanDto {
  id: number;
  userId: number;
  strategyType: number; // 1 = Snowball, 2 = Avalanche
  monthlyBudget: number;
  totalMonths: number;
  totalInterestPaid: number;
  debtFreeDate: string;
  debtPriorityOrder: string[];
  createdAt: string;
}

export interface CalculateStrategyPayload {
  monthlyBudget: number;
  strategyType: number; // 1 = Snowball, 2 = Avalanche
}

// Calcule et sauvegarde un plan de remboursement
// Le Gateway injecte automatiquement l'userId depuis le token JWT
export async function calculateStrategy(
  payload: CalculateStrategyPayload
): Promise<RepaymentPlanDto | null> {
  try {
    const response = await fetch(`${BASE}/api/Strategy/calculate`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify(payload),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to calculate strategy", error);
    return null;
  }
}

// Récupère les plans sauvegardés de l'utilisateur connecté
export async function getUserPlans(): Promise<RepaymentPlanDto[]> {
  try {
    const response = await fetch(`${BASE}/api/Strategy/plans`, {
      headers: authHeaders(),
    });
    if (!response.ok) return [];
    return response.json();
  } catch (error) {
    console.error("Failed to load plans", error);
    return [];
  }
}

// Supprime un plan par son ID
export async function deletePlan(id: number): Promise<boolean> {
  try {
    const response = await fetch(`${BASE}/api/Strategy/plans/${id}`, {
      method: "DELETE",
      headers: authHeaders(),
    });
    return response.ok;
  } catch (error) {
    console.error("Failed to delete plan", error);
    return false;
  }
}
