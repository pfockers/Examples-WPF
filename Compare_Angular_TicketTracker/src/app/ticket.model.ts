export type Priority = 'low' | 'medium' | 'high';
export type Status = 'open' | 'in-progress' | 'done';

export interface Ticket {
  id: number;
  title: string;
  description: string | null;
  priority: Priority;
  status: Status;
  createdAt: string;
}

export const STATUS_LABEL: Record<Status, string> = {
  open: 'Offen',
  'in-progress': 'In Arbeit',
  done: 'Erledigt',
};

export const PRIORITY_LABEL: Record<Priority, string> = {
  low: 'Niedrig',
  medium: 'Mittel',
  high: 'Hoch',
};

export const STATUSES = Object.keys(STATUS_LABEL) as Status[];
export const PRIORITIES = Object.keys(PRIORITY_LABEL) as Priority[];
