import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { environment } from '../environments/environment';
import { Ticket } from './ticket.model';
import { TicketService } from './ticket.service';

const URL = `${environment.apiUrl}/api/tickets`;

const ticket = (id: number, title: string, status: Ticket['status'] = 'open'): Ticket => ({
  id,
  title,
  description: null,
  priority: 'medium',
  status,
  createdAt: '2026-10-09T10:00:00',
});

describe('TicketService', () => {
  let service: TicketService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [provideHttpClient(), provideHttpClientTesting()] });
    service = TestBed.inject(TicketService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('load() fuellt die Liste und beendet den Ladezustand', () => {
    service.load();
    expect(service.loading()).toBeTrue();

    http.expectOne(URL).flush([ticket(1, 'A'), ticket(2, 'B')]);

    expect(service.tickets().length).toBe(2);
    expect(service.loading()).toBeFalse();
  });

  it('load() meldet einen Fehler, wenn das Backend nicht erreichbar ist', () => {
    service.load();
    http.expectOne(URL).error(new ProgressEvent('error'));

    expect(service.loadError()).toContain('nicht erreichbar');
    expect(service.loading()).toBeFalse();
  });

  it('visible() filtert nach Suchtext und Status', () => {
    service.tickets.set([ticket(1, 'Login kaputt'), ticket(2, 'Logo tauschen', 'done')]);

    service.search.set('login');
    expect(service.visible().map((t) => t.id)).toEqual([1]);

    service.search.set('');
    service.filter.set('done');
    expect(service.visible().map((t) => t.id)).toEqual([2]);
  });

  it('add() setzt das neue Ticket an den Anfang der Liste', () => {
    service.tickets.set([ticket(1, 'Alt')]);

    service.add('Neu', 'high').subscribe();
    const req = http.expectOne(URL);
    expect(req.request.method).toBe('POST');
    req.flush(ticket(2, 'Neu'));

    expect(service.tickets().map((t) => t.id)).toEqual([2, 1]);
  });
});
