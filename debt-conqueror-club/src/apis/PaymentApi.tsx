const BASE = "http://localhost:5099";

function authHeaders() {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  };
}

export async function getPayments() {
  try {
    const response = await fetch(`${BASE}/api/v1/Payment`, {
      method: "GET",
      headers: authHeaders(),
    });
    if (!response.ok) return [];
    return response.json();
  } catch (error) {
    console.error("Failed to load Payments", error);
    return [];
  }
}

export async function createPayment(payload: {
  debtId: number;
  amount: number;
  paymentDate: string;
  notes?: string;
}) {
  try {
    const response = await fetch(`${BASE}/api/v1/Payment`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify(payload),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to create Payment", error);
    return null;
  }
}
