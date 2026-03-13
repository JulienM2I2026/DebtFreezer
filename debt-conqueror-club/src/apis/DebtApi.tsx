const BASE = "http://localhost:5099";

function authHeaders() {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  };
}

export async function getDebts() {
  try {
    const response = await fetch(`${BASE}/api/Debt`, {
      method: "GET",
      headers: authHeaders(),
    });
    if (!response.ok) return [];
    return response.json();
  } catch (error) {
    console.error("Failed to load Debts", error);
    return [];
  }
}

export async function createDebt(payload: {
  creditor: string;
  originalAmount: number;
  interestRate: number;
  dueDate: string;
  type: number;
}) {
  try {
    const response = await fetch(`${BASE}/api/Debt`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify(payload),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to create Debt", error);
    return null;
  }
}
