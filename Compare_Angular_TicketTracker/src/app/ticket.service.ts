import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { tap } from 'rxjs';
import { environment } from '../environments/environment';
import { errorMessage } from './api-error';
import { Priority, Status, Ticket } from './ticket.model';

const BASE = `${environment.apiUrl}/api/tickets`;

export interface TicketInput {
  title: string;
  description: string | null;
  priority: Priority;
}

// Service: Zustand und Logik liegen ausserhalb der Komponenten und werden per Dependency Injection geteilt.
@Injectable({ providedIn: 'root' })
export class TicketService {
  private http = inject(HttpClient);

  readonly tickets = signal<Ticket[]>([]);
  readonly loading = signal(false);
  readonly loadError = signal('');
  readonly actionError = signal('');
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

  // HttpClient liefert Observables; subscribe() startet die Anfrage.
  load() {
    this.loading.set(true);
    this.loadError.set('');
    this.http.get<Ticket[]>(BASE).subscribe({
      next: (list) => {
        this.tickets.set(list);
        this.loading.set(false);
      },
      error: (e) => {
        this.loadError.set(errorMessage(e));
        this.loading.set(false);
      },
    });
  }

  get(id: number) {
    return this.http.get<Ticket>(`${BASE}/${id}`);
  }

  update(id: number, input: TicketInput) {
    return this.http.put<Ticket>(`${BASE}/${id}`, input);
  }

  count(status: Status) {
    return this.tickets().filter((t) => t.status === status).length;
  }

  // Gibt das Observable zurueck, damit das Formular Fehler und Ladezustand selbst anzeigen kann.
  add(title: string, priority: Priority) {
    return this.http
      .post<Ticket>(BASE, { title, description: null, priority })
      .pipe(tap((created) => this.tickets.update((list) => [created, ...list])));
  }

  changeStatus(id: number, status: Status) {
    this.actionError.set('');
    this.http.put<Ticket>(`${BASE}/${id}/status`, { status }).subscribe({
      next: (updated) => this.tickets.update((list) => list.map((t) => (t.id === id ? updated : t))),
      error: (e) => this.actionError.set(errorMessage(e)),
    });
  }

  remove(id: number) {
    this.actionError.set('');
    this.http.delete<void>(`${BASE}/${id}`).subscribe({
      next: () => this.tickets.update((list) => list.filter((t) => t.id !== id)),
      error: (e) => this.actionError.set(errorMessage(e)),
    });
  }

  // Fuer die Detailseite: loescht, ohne die Liste zu veraendern (die wird beim Zurueckgehen neu geladen).
  deleteById(id: number) {
    return this.http.delete<void>(`${BASE}/${id}`);
  }
}
