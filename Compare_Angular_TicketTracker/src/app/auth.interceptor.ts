import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { environment } from '../environments/environment';
import { AuthService } from './auth.service';

// Interceptor: haengt das Token an jede API-Anfrage und meldet bei 401 ab (Gegenstueck zu React: zentrale request()-Funktion).
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const isApi = req.url.startsWith(environment.apiUrl);
  const isLogin = req.url.endsWith('/api/auth/login');

  if (isApi && !isLogin && auth.token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${auth.token}` } });
  }

  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401 && !isLogin) auth.logout();
      return throwError(() => err);
    })
  );
};
