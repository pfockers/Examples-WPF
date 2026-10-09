import { DatePipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PRIORITY_LABEL, STATUSES, STATUS_LABEL, Status, Ticket } from './ticket.model';

@Component({
  selector: 'app-ticket-item',
  standalone: true,
  imports: [FormsModule, DatePipe, RouterLink],
  templateUrl: './ticket-item.component.html',
})
export class TicketItemComponent {
  // @Input / @Output entsprechen den React-Props bzw. Callback-Props.
  @Input({ required: true }) ticket!: Ticket;
  @Output() statusChange = new EventEmitter<Status>();
  @Output() delete = new EventEmitter<void>();

  statuses = STATUSES;
  statusLabel = STATUS_LABEL;
  priorityLabel = PRIORITY_LABEL;
}
