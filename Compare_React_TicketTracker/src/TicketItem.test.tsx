import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MemoryRouter } from 'react-router-dom';
import { describe, expect, it, vi } from 'vitest';
import TicketItem from './TicketItem';
import { Ticket } from './types';

const ticket: Ticket = {
  id: 7,
  title: 'Login-Seite zeigt Fehler',
  description: null,
  priority: 'high',
  status: 'open',
  createdAt: '2026-10-09T10:00:00',
};

describe('TicketItem', () => {
  it('zeigt Titel als Link zur Detailseite', () => {
    render(
      <MemoryRouter>
        <TicketItem ticket={ticket} onStatusChange={vi.fn()} onDelete={vi.fn()} />
      </MemoryRouter>
    );
    expect(screen.getByRole('link', { name: ticket.title })).toHaveAttribute('href', '/tickets/7');
  });

  it('meldet Statuswechsel und Loeschen an den Aufrufer', async () => {
    const onStatusChange = vi.fn();
    const onDelete = vi.fn();
    render(
      <MemoryRouter>
        <TicketItem ticket={ticket} onStatusChange={onStatusChange} onDelete={onDelete} />
      </MemoryRouter>
    );

    await userEvent.selectOptions(screen.getByLabelText('Status'), 'done');
    await userEvent.click(screen.getByRole('button', { name: 'Loeschen' }));

    expect(onStatusChange).toHaveBeenCalledWith(7, 'done');
    expect(onDelete).toHaveBeenCalledWith(7);
  });
});
