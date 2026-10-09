import { useCallback, useEffect, useMemo, useState } from 'react';
import { api } from './api';
import TicketForm from './TicketForm';
import TicketItem from './TicketItem';
import { STATUS_LABEL, Status, Ticket } from './types';

export default function TicketsPage() {
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState('');
  const [actionError, setActionError] = useState('');
  const [search, setSearch] = useState('');
  const [filter, setFilter] = useState<Status | 'all'>('all');

  const load = useCallback(() => {
    setLoading(true);
    setLoadError('');
    api
      .list()
      .then(setTickets)
      .catch((e: Error) => setLoadError(e.message))
      .finally(() => setLoading(false));
  }, []);

  // Seiteneffekt: einmal beim Start die Tickets vom Backend laden.
  useEffect(load, [load]);

  const visible = useMemo(
    () =>
      tickets.filter(
        (t) =>
          (filter === 'all' || t.status === filter) &&
          t.title.toLowerCase().includes(search.toLowerCase())
      ),
    [tickets, search, filter]
  );

  const count = (s: Status) => tickets.filter((t) => t.status === s).length;

  // Gemeinsamer Wrapper: Fehler anzeigen, statt sie zu verschlucken.
  const run = async (action: () => Promise<void>) => {
    setActionError('');
    try {
      await action();
    } catch (e) {
      setActionError((e as Error).message);
    }
  };

  const addTicket = (title: string, priority: Ticket['priority']) =>
    run(async () => {
      const created = await api.create({ title, description: null, priority });
      setTickets((list) => [created, ...list]);
    });

  const changeStatus = (id: number, status: Status) =>
    run(async () => {
      const updated = await api.setStatus(id, status);
      setTickets((list) => list.map((t) => (t.id === id ? updated : t)));
    });

  const removeTicket = (id: number) =>
    run(async () => {
      await api.remove(id);
      setTickets((list) => list.filter((t) => t.id !== id));
    });

  return (
    <>
      {actionError && <p className="error">{actionError}</p>}

      <div className="stats">
        {(Object.keys(STATUS_LABEL) as Status[]).map((s) => (
          <div key={s} className={`stat ${s}`}>
            <strong>{count(s)}</strong>
            <span>{STATUS_LABEL[s]}</span>
          </div>
        ))}
      </div>

      <TicketForm onAdd={addTicket} />

      <div className="toolbar">
        <input
          type="search"
          placeholder="Tickets durchsuchen..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <select aria-label="Filter" value={filter} onChange={(e) => setFilter(e.target.value as Status | 'all')}>
          <option value="all">Alle</option>
          {(Object.keys(STATUS_LABEL) as Status[]).map((s) => (
            <option key={s} value={s}>
              {STATUS_LABEL[s]}
            </option>
          ))}
        </select>
      </div>

      {loading && <p className="empty">Lade Tickets...</p>}
      {loadError && (
        <p className="error">
          {loadError} <button onClick={load}>Erneut versuchen</button>
        </p>
      )}

      <ul className="tickets">
        {visible.map((t) => (
          <TicketItem key={t.id} ticket={t} onStatusChange={changeStatus} onDelete={removeTicket} />
        ))}
      </ul>
      {!loading && !loadError && visible.length === 0 && <p className="empty">Keine Tickets gefunden.</p>}
    </>
  );
}
