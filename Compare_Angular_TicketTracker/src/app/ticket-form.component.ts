import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { errorMessage } from './api-error';
import { PRIORITIES, PRIORITY_LABEL } from './ticket.model';
import { TicketService } from './ticket.service';

@Component({
  selector: 'app-ticket-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './ticket-form.component.html',
})
export class TicketFormComponent {
  private service = inject(TicketService);

  priorities = PRIORITIES;
  label = PRIORITY_LABEL;
  busy = false;
  submitted = false;

  // Reactive Forms: Validierungsregeln stehen in der Klasse, nicht im Template.
  form = inject(FormBuilder).nonNullable.group({
    title: ['', [Validators.required, Validators.pattern(/\S/), Validators.minLength(3), Validators.maxLength(200)]],
    priority: 'medium' as 'low' | 'medium' | 'high',
  });

  get title() {
    return this.form.controls.title;
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) return;
    this.busy = true;
    const { title, priority } = this.form.getRawValue();
    this.service.actionError.set('');
    this.service.add(title.trim(), priority).subscribe({
      next: () => {
        this.form.reset({ title: '', priority });
        this.submitted = false;
        this.busy = false;
      },
      error: (e) => {
        this.service.actionError.set(errorMessage(e));
        this.busy = false;
      },
    });
  }
}
