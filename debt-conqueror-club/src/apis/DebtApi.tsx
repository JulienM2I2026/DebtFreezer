const BASE = "http://localhost:5099";

function authHeaders() {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  };
}

export async function getDebts() {
  try {
    const response = await fetch(`${BASE}/api/v1/Debt`, {
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
    const response = await fetch(`${BASE}/api/v1/Debt`, {
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

export async function patchDebt(id: number, payload: {
  creditor?: string;
  originalAmount?: number;
  remainingAmount?: number;
  interestRate?: number;
  dueDate?: string;
  type?: number;
  status?: number;
}) {
  try {
    const response = await fetch(`${BASE}/api/v1/Debt/${id}`, {
      method: "PATCH",
      headers: authHeaders(),
      body: JSON.stringify(payload),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to patch Debt", error);
    return null;
  }
}

export async function deleteDebt(id: number): Promise<boolean> {
  try {
    const response = await fetch(`${BASE}/api/v1/Debt/${id}`, {
      method: "DELETE",
      headers: authHeaders(),
    });
    return response.ok;
  } catch (error) {
    console.error("Failed to delete Debt", error);
    return false;
  }
}

export async function getDebtSummary() {
  try {
    const response = await fetch(`${BASE}/api/v1/Debt/summary`, {
      method: "GET",
      headers: authHeaders(),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to load Debt summary", error);
    return null;
  }
}

export async function getDebtByMonth() {
  try {
    const response = await fetch(`${BASE}/api/v1/Debt/by-month`, {
      method: "GET",
      headers: authHeaders(),
    });
    if (!response.ok) return [];
    return response.json();
  } catch (error) {
    console.error("Failed to load Debt by month", error);
    return [];
  }
}

export async function getPagedDebts(
  page = 1,
  pageSize = 10,
  status?: number,
  type?: number,
  creditor?: string
) {
  try {
    let qs = `?page=${page}&pageSize=${pageSize}`;
    if (status !== undefined) qs += `&status=${status}`;
    if (type !== undefined) qs += `&type=${type}`;
    if (creditor) qs += `&creditor=${encodeURIComponent(creditor)}`;
    const response = await fetch(`${BASE}/api/v1/Debt/paged${qs}`, {
      method: "GET",
      headers: authHeaders(),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to load paged Debts", error);
    return null;
  }
}
