import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { STATUSES, STATUS_LABEL } from './ticket.model';
import { TicketFormComponent } from './ticket-form.component';
import { TicketItemComponent } from './ticket-item.component';
import { TicketService } from './ticket.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, TicketFormComponent, TicketItemComponent],
  templateUrl: './app.component.html',
})
export class AppComponent {
  service = inject(TicketService);

  statuses = STATUSES;
  statusLabel = STATUS_LABEL;
}
