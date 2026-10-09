import { FormEvent, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from './auth';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [username, setUsername] = useState('demo');
  const [password, setPassword] = useState('');
  const [submitted, setSubmitted] = useState(false);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
    setError('');
    if (!username.trim() || !password) return;
    setBusy(true);
    try {
      await login(username.trim(), password);
      navigate((location.state as { from?: string } | null)?.from ?? '/', { replace: true });
    } catch (err) {
      setError((err as { status?: number }).status === 401 ? 'Benutzername oder Passwort falsch.' : (err as Error).message);
    } finally {
      setBusy(false);
    }
  };

  return (
    <form className="card stack" onSubmit={submit} noValidate>
      <h2>Anmelden</h2>
      <p className="hint">Demo-Zugang: demo / demo123</p>
      {error && <p className="error">{error}</p>}
      <label>
        Benutzername
        <input value={username} onChange={(e) => setUsername(e.target.value)} autoComplete="username" />
        {submitted && !username.trim() && <span className="field-error">Benutzername ist erforderlich.</span>}
      </label>
      <label>
        Passwort
        <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} autoComplete="current-password" />
        {submitted && !password && <span className="field-error">Passwort ist erforderlich.</span>}
      </label>
      <button type="submit" className="primary" disabled={busy}>
        {busy ? 'Anmelden...' : 'Anmelden'}
      </button>
    </form>
  );
}
