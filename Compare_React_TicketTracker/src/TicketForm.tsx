import { FormEvent, useState } from 'react';
import { PRIORITY_LABEL, Priority } from './types';

type Props = { onAdd: (title: string, priority: Priority) => Promise<void> | void };

export function validateTitle(title: string): string {
  const t = title.trim();
  if (!t) return 'Titel ist erforderlich.';
  if (t.length < 3) return 'Titel muss mindestens 3 Zeichen lang sein.';
  if (t.length > 200) return 'Titel darf hoechstens 200 Zeichen lang sein.';
  return '';
}

export default function TicketForm({ onAdd }: Props) {
  const [title, setTitle] = useState('');
  const [priority, setPriority] = useState<Priority>('medium');
  const [touched, setTouched] = useState(false);
  const [busy, setBusy] = useState(false);

  // Validierung von Hand: der Fehlertext wird aus dem aktuellen State abgeleitet.
  const error = touched ? validateTitle(title) : '';

  const submit = async (e: FormEvent) => {
    e.preventDefault();
    setTouched(true);
    if (validateTitle(title)) return;
    setBusy(true);
    try {
      await onAdd(title.trim(), priority);
      setTitle('');
      setTouched(false);
    } finally {
      setBusy(false);
    }
  };

  return (
    <form className="form" onSubmit={submit} noValidate>
      <div className="field">
        <input
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          onBlur={() => setTouched(true)}
          placeholder="Neues Ticket..."
          aria-invalid={!!error}
        />
        {error && <span className="field-error">{error}</span>}
      </div>
      <select value={priority} onChange={(e) => setPriority(e.target.value as Priority)}>
        {(Object.keys(PRIORITY_LABEL) as Priority[]).map((p) => (
          <option key={p} value={p}>
            {PRIORITY_LABEL[p]}
          </option>
        ))}
      </select>
      <button type="submit" className="primary" disabled={busy}>
        {busy ? 'Speichern...' : 'Hinzufuegen'}
      </button>
    </form>
  );
}
