import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../environments/environment';

export interface Session {
  token: string;
  username: string;
}

const KEY = 'auth';

// Das Token liegt in sessionStorage und verschwindet beim Schliessen des Tabs.
@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  readonly session = signal<Session | null>(JSON.parse(sessionStorage.getItem(KEY) ?? 'null'));
  readonly username = computed(() => this.session()?.username ?? null);

  get token() {
    return this.session()?.token ?? null;
  }

  login(username: string, password: string) {
    return this.http
      .post<Session>(`${environment.apiUrl}/api/auth/login`, { username, password })
      .pipe(
        tap((s) => {
          const session = { token: s.token, username: s.username };
          sessionStorage.setItem(KEY, JSON.stringify(session));
          this.session.set(session);
        })
      );
  }

  logout() {
    sessionStorage.removeItem(KEY);
    this.session.set(null);
    this.router.navigate(['/login']);
  }
}
