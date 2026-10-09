import { DatePipe } from '@angular/common';
import { Component, Input, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { errorMessage } from './api-error';
import { PRIORITIES, PRIORITY_LABEL, STATUS_LABEL, Ticket } from './ticket.model';
import { TicketService } from './ticket.service';

@Component({
  selector: 'app-ticket-detail',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './ticket-detail.component.html',
})
export class TicketDetailComponent implements OnInit {
  private service = inject(TicketService);
  private router = inject(Router);

  // Route-Parameter werden per withComponentInputBinding() direkt als @Input geliefert.
  @Input() id!: string;

  ticket: Ticket | null = null;
  error = '';
  saved = false;
  submitted = false;

  priorities = PRIORITIES;
  priorityLabel = PRIORITY_LABEL;
  statusLabel = STATUS_LABEL;

  form = inject(FormBuilder).nonNullable.group({
    title: ['', [Validators.required, Validators.pattern(/\S/), Validators.minLength(3), Validators.maxLength(200)]],
    description: ['', Validators.maxLength(1000)],
    priority: 'medium' as 'low' | 'medium' | 'high',
  });

  ngOnInit() {
    this.service.get(Number(this.id)).subscribe({
      next: (t) => {
        this.ticket = t;
        this.form.setValue({ title: t.title, description: t.description ?? '', priority: t.priority });
      },
      error: (e) => (this.error = e.status === 404 ? 'Ticket nicht gefunden.' : errorMessage(e)),
    });
  }

  save() {
    this.submitted = true;
    this.saved = false;
    if (this.form.invalid) return;
    const { title, description, priority } = this.form.getRawValue();
    this.service.update(Number(this.id), { title: title.trim(), description: description.trim() || null, priority }).subscribe({
      next: (t) => {
        this.ticket = t;
        this.saved = true;
        this.error = '';
      },
      error: (e) => (this.error = errorMessage(e)),
    });
  }

  remove() {
    this.service.deleteById(Number(this.id)).subscribe({
      next: () => this.router.navigate(['/']),
      error: (e) => (this.error = errorMessage(e)),
    });
  }
}
