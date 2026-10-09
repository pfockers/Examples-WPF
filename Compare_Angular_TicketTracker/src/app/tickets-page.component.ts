import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { STATUSES, STATUS_LABEL } from './ticket.model';
import { TicketFormComponent } from './ticket-form.component';
import { TicketItemComponent } from './ticket-item.component';
import { TicketService } from './ticket.service';

@Component({
  selector: 'app-tickets-page',
  standalone: true,
  imports: [FormsModule, TicketFormComponent, TicketItemComponent],
  templateUrl: './tickets-page.component.html',
})
export class TicketsPageComponent implements OnInit {
  service = inject(TicketService);

  statuses = STATUSES;
  statusLabel = STATUS_LABEL;

  // Lifecycle-Hook (Gegenstueck zu useEffect(..., []) und OnInitializedAsync): einmal beim Start laden.
  ngOnInit() {
    this.service.load();
  }
}
