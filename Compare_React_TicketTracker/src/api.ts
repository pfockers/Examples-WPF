import { Priority, Status, Ticket } from './types';
import { Session, tokenStore } from './tokenStore';

const BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5300';

export class ApiError extends Error {
  constructor(
    public status: number,
    message: string
  ) {
    super(message);
  }
}

let onUnauthorized: () => void = () => {};
export const setUnauthorizedHandler = (fn: () => void) => {
  onUnauthorized = fn;
};

async function errorMessage(res: Response): Promise<string> {
  try {
    const body = await res.json();
    if (body?.errors) return Object.values<string[]>(body.errors).flat().join(' ');
    return body?.detail ?? body?.title ?? `HTTP ${res.status}`;
  } catch {
    return `HTTP ${res.status}`;
  }
}

async function request<T>(path: string, init: RequestInit = {}, auth = true): Promise<T> {
  const headers: Record<string, string> = { 'Content-Type': 'application/json' };
  const session = tokenStore.get();
  if (auth && session) headers.Authorization = `Bearer ${session.token}`;

  let res: Response;
  try {
    res = await fetch(`${BASE}${path}`, { ...init, headers });
  } catch {
    throw new ApiError(0, `Backend nicht erreichbar (${BASE}).`);
  }

  if (res.status === 401 && auth) {
    tokenStore.clear();
    onUnauthorized();
  }
  if (!res.ok) throw new ApiError(res.status, await errorMessage(res));
  return res.status === 204 ? (undefined as T) : res.json();
}

type TicketInput = { title: string; description: string | null; priority: Priority };

export const api = {
  login: (username: string, password: string) =>
    request<Session & { expiresAt: string }>('/api/auth/login', { method: 'POST', body: JSON.stringify({ username, password }) }, false),
  list: () => request<Ticket[]>('/api/tickets'),
  get: (id: number) => request<Ticket>(`/api/tickets/${id}`),
  create: (input: TicketInput) => request<Ticket>('/api/tickets', { method: 'POST', body: JSON.stringify(input) }),
  update: (id: number, input: TicketInput) =>
    request<Ticket>(`/api/tickets/${id}`, { method: 'PUT', body: JSON.stringify(input) }),
  setStatus: (id: number, status: Status) =>
    request<Ticket>(`/api/tickets/${id}/status`, { method: 'PUT', body: JSON.stringify({ status }) }),
  remove: (id: number) => request<void>(`/api/tickets/${id}`, { method: 'DELETE' }),
};
