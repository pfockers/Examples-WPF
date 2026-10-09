import { createContext, ReactNode, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { api, setUnauthorizedHandler } from './api';
import { tokenStore } from './tokenStore';

type AuthState = {
  username: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
};

// Context: Gegenstueck zu Angular-Services/Blazor-@inject, um Zustand ohne Props durchzureichen.
const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [username, setUsername] = useState<string | null>(() => tokenStore.get()?.username ?? null);

  const logout = useCallback(() => {
    tokenStore.clear();
    setUsername(null);
  }, []);

  const login = useCallback(async (user: string, password: string) => {
    const session = await api.login(user, password);
    tokenStore.set({ token: session.token, username: session.username });
    setUsername(session.username);
  }, []);

  // Bei 401 vom Server automatisch abmelden.
  useEffect(() => setUnauthorizedHandler(logout), [logout]);

  const value = useMemo(() => ({ username, login, logout }), [username, login, logout]);
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth braucht einen AuthProvider');
  return ctx;
}

// Route Guard: nicht angemeldete Benutzer landen auf /login.
export function RequireAuth({ children }: { children: ReactNode }) {
  const { username } = useAuth();
  const location = useLocation();
  return username ? <>{children}</> : <Navigate to="/login" replace state={{ from: location.pathname }} />;
}
