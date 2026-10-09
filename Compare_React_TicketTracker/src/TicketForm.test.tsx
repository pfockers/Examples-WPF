import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import TicketForm, { validateTitle } from './TicketForm';

describe('validateTitle', () => {
  it('lehnt leere und zu kurze Titel ab', () => {
    expect(validateTitle('  ')).toMatch(/erforderlich/);
    expect(validateTitle('ab')).toMatch(/mindestens 3/);
    expect(validateTitle('abc')).toBe('');
  });
});

describe('TicketForm', () => {
  it('zeigt einen Fehler und ruft onAdd nicht auf, wenn der Titel zu kurz ist', async () => {
    const onAdd = vi.fn();
    render(<TicketForm onAdd={onAdd} />);

    await userEvent.type(screen.getByPlaceholderText('Neues Ticket...'), 'ab');
    await userEvent.click(screen.getByRole('button', { name: 'Hinzufuegen' }));

    expect(screen.getByText(/mindestens 3/)).toBeInTheDocument();
    expect(onAdd).not.toHaveBeenCalled();
  });

  it('ruft onAdd mit Titel und Prioritaet auf und leert das Feld', async () => {
    const onAdd = vi.fn().mockResolvedValue(undefined);
    render(<TicketForm onAdd={onAdd} />);

    const input = screen.getByPlaceholderText('Neues Ticket...');
    await userEvent.type(input, 'Drucker defekt');
    await userEvent.selectOptions(screen.getByRole('combobox'), 'high');
    await userEvent.click(screen.getByRole('button', { name: 'Hinzufuegen' }));

    expect(onAdd).toHaveBeenCalledWith('Drucker defekt', 'high');
    expect(input).toHaveValue('');
  });
});
