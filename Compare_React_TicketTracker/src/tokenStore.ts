// Das Token liegt in sessionStorage und verschwindet beim Schliessen des Tabs.
const KEY = 'auth';

export type Session = { token: string; username: string };

export const tokenStore = {
  get(): Session | null {
    const raw = sessionStorage.getItem(KEY);
    return raw ? (JSON.parse(raw) as Session) : null;
  },
  set(session: Session) {
    sessionStorage.setItem(KEY, JSON.stringify(session));
  },
  clear() {
    sessionStorage.removeItem(KEY);
  },
};
