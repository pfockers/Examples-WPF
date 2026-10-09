import { FormEvent, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { api } from './api';
import { validateTitle } from './TicketForm';
import { PRIORITY_LABEL, Priority, STATUS_LABEL, Ticket } from './types';

export default function TicketDetailPage() {
  // Route-Parameter: Gegenstueck zu ActivatedRoute (Angular) bzw. [Parameter] mit @page (Blazor).
  const { id } = useParams();
  const navigate = useNavigate();

  const [ticket, setTicket] = useState<Ticket | null>(null);
  const [error, setError] = useState('');
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [priority, setPriority] = useState<Priority>('medium');
  const [submitted, setSubmitted] = useState(false);
  const [saved, setSaved] = useState(false);

  useEffect(() => {
    api
      .get(Number(id))
      .then((t) => {
        setTicket(t);
        setTitle(t.title);
        setDescription(t.description ?? '');
        setPriority(t.priority);
      })
      .catch((e: Error & { status?: number }) => setError(e.status === 404 ? 'Ticket nicht gefunden.' : e.message));
  }, [id]);

  const titleError = submitted ? validateTitle(title) : '';
  const descriptionError = description.length > 1000 ? 'Beschreibung darf hoechstens 1000 Zeichen lang sein.' : '';

  const save = async (e: FormEvent) => {
    e.preventDefault();
    setSubmitted(true);
    setSaved(false);
    if (validateTitle(title) || descriptionError) return;
    try {
      setTicket(await api.update(Number(id), { title: title.trim(), description: description.trim() || null, priority }));
      setSaved(true);
      setError('');
    } catch (err) {
      setError((err as Error).message);
    }
  };

  const remove = async () => {
    try {
      await api.remove(Number(id));
      navigate('/');
    } catch (err) {
      setError((err as Error).message);
    }
  };

  if (!ticket) return error ? <p className="error">{error}</p> : <p className="empty">Lade Ticket...</p>;

  return (
    <section className="detail">
      <Link to="/">&larr; Zurueck zur Liste</Link>
      <h2>Ticket #{ticket.id}</h2>
      <p className="meta">
        <span className={`priority ${ticket.priority}`}>{PRIORITY_LABEL[ticket.priority]}</span>{' '}
        {STATUS_LABEL[ticket.status]} &middot; erstellt am {new Date(ticket.createdAt).toLocaleDateString('de-DE')}
      </p>

      {error && <p className="error">{error}</p>}
      {saved && <p className="success">Gespeichert.</p>}

      <form className="stack" onSubmit={save} noValidate>
        <label>
          Titel
          <input value={title} onChange={(e) => setTitle(e.target.value)} aria-invalid={!!titleError} />
          {titleError && <span className="field-error">{titleError}</span>}
        </label>
        <label>
          Beschreibung
          <textarea rows={5} value={description} onChange={(e) => setDescription(e.target.value)} />
          {descriptionError && <span className="field-error">{descriptionError}</span>}
        </label>
        <label>
          Prioritaet
          <select value={priority} onChange={(e) => setPriority(e.target.value as Priority)}>
            {(Object.keys(PRIORITY_LABEL) as Priority[]).map((p) => (
              <option key={p} value={p}>
                {PRIORITY_LABEL[p]}
              </option>
            ))}
          </select>
        </label>
        <div className="actions">
          <button type="submit" className="primary">Speichern</button>
          <button type="button" className="danger" onClick={remove}>Loeschen</button>
        </div>
      </form>
    </section>
  );
}
