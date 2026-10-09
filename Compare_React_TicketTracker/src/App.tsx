import { useEffect, useMemo, useState } from 'react';
import TicketForm from './TicketForm';
import TicketItem from './TicketItem';
import { api } from './api';
import { STATUS_LABEL, Status, Ticket } from './types';

export default function App() {
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [search, setSearch] = useState('');
  const [filter, setFilter] = useState<Status | 'all'>('all');
  const [error, setError] = useState('');

  // Seiteneffekt: einmal beim Start die Tickets vom Backend laden.
  useEffect(() => {
    api.list().then(setTickets).catch(() => setError('Backend nicht erreichbar (http://localhost:5300).'));
  }, []);

  // Abgeleitete Werte werden mit useMemo gecacht.
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

  const addTicket = async (title: string, priority: Ticket['priority']) => {
    const created = await api.create(title, priority);
    setTickets((list) => [created, ...list]);
  };

  const changeStatus = async (id: number, status: Status) => {
    const updated = await api.setStatus(id, status);
    setTickets((list) => list.map((t) => (t.id === id ? updated : t)));
  };

  const removeTicket = async (id: number) => {
    await api.remove(id);
    setTickets((list) => list.filter((t) => t.id !== id));
  };

  return (
    <div className="app">
      <header>
        <h1>Ticket Tracker</h1>
        <span className="badge-tech react">React</span>
      </header>

      {error && <p className="error">{error}</p>}

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
        <select value={filter} onChange={(e) => setFilter(e.target.value as Status | 'all')}>
          <option value="all">Alle</option>
          {(Object.keys(STATUS_LABEL) as Status[]).map((s) => (
            <option key={s} value={s}>
              {STATUS_LABEL[s]}
            </option>
          ))}
        </select>
      </div>

      <ul className="tickets">
        {visible.map((t) => (
          <TicketItem key={t.id} ticket={t} onStatusChange={changeStatus} onDelete={removeTicket} />
        ))}
      </ul>
      {visible.length === 0 && <p className="empty">Keine Tickets gefunden.</p>}
    </div>
  );
}
