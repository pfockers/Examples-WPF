import { Link } from 'react-router-dom';
import { PRIORITY_LABEL, STATUS_LABEL, Status, Ticket } from './types';

type Props = {
  ticket: Ticket;
  onStatusChange: (id: number, status: Status) => void;
  onDelete: (id: number) => void;
};

export default function TicketItem({ ticket, onStatusChange, onDelete }: Props) {
  return (
    <li className={`ticket ${ticket.status}`}>
      <div className="ticket-main">
        <span className={`priority ${ticket.priority}`}>{PRIORITY_LABEL[ticket.priority]}</span>
        <Link className="title" to={`/tickets/${ticket.id}`}>
          {ticket.title}
        </Link>
        <small>{new Date(ticket.createdAt).toLocaleDateString('de-DE')}</small>
      </div>
      <div className="ticket-actions">
        <select
          aria-label="Status"
          value={ticket.status}
          onChange={(e) => onStatusChange(ticket.id, e.target.value as Status)}
        >
          {(Object.keys(STATUS_LABEL) as Status[]).map((s) => (
            <option key={s} value={s}>
              {STATUS_LABEL[s]}
            </option>
          ))}
        </select>
        <button className="danger" onClick={() => onDelete(ticket.id)} aria-label="Loeschen">
          &times;
        </button>
      </div>
    </li>
  );
}
