const BASE = "http://localhost:5099";

function authHeaders() {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${localStorage.getItem("token")}`,
  };
}

export interface ChallengeDto {
  id: number;
  title: string;
  description?: string;
  targetAmount: number;
  totalPaid: number;
  progressPercent: number;
  dueDate: string;
  creatorUserId: number;
  participantCount: number;
  status: number; // 0 = Active, 1 = Completed
  createdAt: string;
}

export interface ChallengeProgressDto {
  challengeId: number;
  title: string;
  targetAmount: number;
  totalPaid: number;
  progressPercent: number;
  participantCount: number;
  status: number;
  dueDate: string;
  leaderboard: LeaderboardEntryDto[];
}

export interface LeaderboardEntryDto {
  rank: number;
  userId: number;
  amountPaid: number;
  joinedAt: string;
}

export interface CreateChallengePayload {
  title: string;
  description?: string;
  targetAmount: number;
  dueDate: string;
  creatorUserId: string | number;
}

export async function getChallenges(): Promise<ChallengeDto[]> {
  try {
    const response = await fetch(`${BASE}/api/v1/Challenge`, {
      headers: authHeaders(),
    });
    if (!response.ok) return [];
    return response.json();
  } catch (error) {
    console.error("Failed to load challenges", error);
    return [];
  }
}

export async function createChallenge(
  payload: CreateChallengePayload
): Promise<ChallengeDto | null> {
  try {
    const response = await fetch(`${BASE}/api/v1/Challenge`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify(payload),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to create challenge", error);
    return null;
  }
}

export async function joinChallenge(
  id: number,
  userId: string | null
): Promise<boolean> {
  try {
    const response = await fetch(`${BASE}/api/v1/Challenge/${id}/join`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify({ userId }),
    });
    return response.ok;
  } catch (error) {
    console.error("Failed to join challenge", error);
    return false;
  }
}

export async function recordChallengePayment(
  challengeId: number,
  paymentId: number,
  userId: string | null
): Promise<boolean> {
  try {
    const response = await fetch(`${BASE}/api/v1/Challenge/${challengeId}/payment`, {
      method: "POST",
      headers: authHeaders(),
      body: JSON.stringify({ paymentId, userId }),
    });
    return response.ok;
  } catch (error) {
    console.error("Failed to record challenge payment", error);
    return false;
  }
}

export async function getChallengeProgress(
  id: number
): Promise<ChallengeProgressDto | null> {
  try {
    const response = await fetch(`${BASE}/api/v1/Challenge/${id}/progress`, {
      headers: authHeaders(),
    });
    if (!response.ok) return null;
    return response.json();
  } catch (error) {
    console.error("Failed to load challenge progress", error);
    return null;
  }
}

export async function getLeaderboard(id: number): Promise<LeaderboardEntryDto[]> {
  try {
    const response = await fetch(`${BASE}/api/v1/Challenge/${id}/leaderboard`, {
      headers: authHeaders(),
    });
    if (!response.ok) return [];
    return response.json();
  } catch (error) {
    console.error("Failed to load leaderboard", error);
    return [];
  }
}
