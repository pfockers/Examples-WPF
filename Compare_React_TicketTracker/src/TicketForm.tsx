import { FormEvent, useState } from 'react';
import { PRIORITY_LABEL, Priority } from './types';

type Props = { onAdd: (title: string, priority: Priority) => void };

export default function TicketForm({ onAdd }: Props) {
  const [title, setTitle] = useState('');
  const [priority, setPriority] = useState<Priority>('medium');

  const submit = (e: FormEvent) => {
    e.preventDefault();
    if (!title.trim()) return;
    onAdd(title.trim(), priority);
    setTitle('');
  };

  return (
    <form className="form" onSubmit={submit}>
      <input
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Neues Ticket..."
      />
      <select value={priority} onChange={(e) => setPriority(e.target.value as Priority)}>
        {(Object.keys(PRIORITY_LABEL) as Priority[]).map((p) => (
          <option key={p} value={p}>
            {PRIORITY_LABEL[p]}
          </option>
        ))}
      </select>
      <button type="submit" className="primary" disabled={!title.trim()}>
        Hinzufuegen
      </button>
    </form>
  );
}
