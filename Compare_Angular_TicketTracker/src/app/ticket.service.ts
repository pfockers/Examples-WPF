import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Priority, Status, Ticket } from './ticket.model';

const BASE = 'http://localhost:5300/api/tickets';

// Service: Zustand und Logik liegen ausserhalb der Komponenten und werden per Dependency Injection geteilt.
@Injectable({ providedIn: 'root' })
export class TicketService {
  private http = inject(HttpClient);

  readonly tickets = signal<Ticket[]>([]);
  readonly error = signal('');
  readonly search = signal('');
  readonly filter = signal<Status | 'all'>('all');

  // computed() cacht abgeleitete Werte automatisch (Gegenstueck zu useMemo).
  readonly visible = computed(() =>
    this.tickets().filter(
      (t) =>
        (this.filter() === 'all' || t.status === this.filter()) &&
        t.title.toLowerCase().includes(this.search().toLowerCase())
    )
  );

  constructor() {
    // HttpClient liefert Observables; subscribe() startet die Anfrage.
    this.http.get<Ticket[]>(BASE).subscribe({
      next: (list) => this.tickets.set(list),
      error: () => this.error.set('Backend nicht erreichbar (http://localhost:5300).'),
    });
  }

  count(status: Status) {
    return this.tickets().filter((t) => t.status === status).length;
  }

  add(title: string, priority: Priority) {
    this.http
      .post<Ticket>(BASE, { title, priority })
      .subscribe((created) => this.tickets.update((list) => [created, ...list]));
  }

  changeStatus(id: number, status: Status) {
    this.http
      .put<Ticket>(`${BASE}/${id}/status`, { status })
      .subscribe((updated) => this.tickets.update((list) => list.map((t) => (t.id === id ? updated : t))));
  }

  remove(id: number) {
    this.http
      .delete<void>(`${BASE}/${id}`)
      .subscribe(() => this.tickets.update((list) => list.filter((t) => t.id !== id)));
  }
}
