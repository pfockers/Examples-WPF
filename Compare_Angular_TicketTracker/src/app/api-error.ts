import { HttpErrorResponse } from '@angular/common/http';
import { environment } from '../environments/environment';

export function errorMessage(e: unknown): string {
  if (e instanceof HttpErrorResponse) {
    if (e.status === 0) return `Backend nicht erreichbar (${environment.apiUrl}).`;
    const body = e.error;
    if (body?.errors) return (Object.values(body.errors) as string[][]).flat().join(' ');
    return body?.detail ?? body?.title ?? `HTTP ${e.status}`;
  }
  return 'Unbekannter Fehler.';
}
