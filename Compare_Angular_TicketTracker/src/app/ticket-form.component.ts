import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PRIORITIES, PRIORITY_LABEL, Priority } from './ticket.model';
import { TicketService } from './ticket.service';

@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './ticket-form.component.html',
})
export class TicketFormComponent {
  private service = inject(TicketService);

  priorities = PRIORITIES;
  label = PRIORITY_LABEL;
  title = '';
  priority: Priority = 'medium';

  submit() {
    if (!this.title.trim()) return;
    this.service.add(this.title.trim(), this.priority);
    this.title = '';
  }
}
