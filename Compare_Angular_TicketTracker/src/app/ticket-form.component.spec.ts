import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TicketFormComponent } from './ticket-form.component';

describe('TicketFormComponent', () => {
  let fixture: ComponentFixture<TicketFormComponent>;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [TicketFormComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    fixture = TestBed.createComponent(TicketFormComponent);
    http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  const el = () => fixture.nativeElement as HTMLElement;

  it('zeigt einen Fehler und sendet nichts, wenn der Titel zu kurz ist', () => {
    const input = el().querySelector('input') as HTMLInputElement;
    input.value = 'ab';
    input.dispatchEvent(new Event('input'));
    (el().querySelector('form') as HTMLFormElement).dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(el().querySelector('.field-error')?.textContent).toContain('mindestens 3');
    http.expectNone(() => true);
  });

  it('sendet ein gueltiges Ticket an die API', () => {
    const input = el().querySelector('input') as HTMLInputElement;
    input.value = 'Drucker defekt';
    input.dispatchEvent(new Event('input'));
    (el().querySelector('form') as HTMLFormElement).dispatchEvent(new Event('submit'));

    const req = http.expectOne((r) => r.method === 'POST');
    expect(req.request.body).toEqual({ title: 'Drucker defekt', description: null, priority: 'medium' });
  });
});
