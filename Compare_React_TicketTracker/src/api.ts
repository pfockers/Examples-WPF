import { Priority, Status, Ticket } from './types';

const BASE = 'http://localhost:5300/api/tickets';

async function request<T>(url: string, init?: RequestInit): Promise<T> {
  const res = await fetch(url, { headers: { 'Content-Type': 'application/json' }, ...init });
  if (!res.ok) throw new Error(`HTTP ${res.status}`);
  return res.status === 204 ? (undefined as T) : res.json();
}

export const api = {
  list: () => request<Ticket[]>(BASE),
  create: (title: string, priority: Priority) =>
    request<Ticket>(BASE, { method: 'POST', body: JSON.stringify({ title, priority }) }),
  setStatus: (id: number, status: Status) =>
    request<Ticket>(`${BASE}/${id}/status`, { method: 'PUT', body: JSON.stringify({ status }) }),
  remove: (id: number) => request<void>(`${BASE}/${id}`, { method: 'DELETE' }),
};
