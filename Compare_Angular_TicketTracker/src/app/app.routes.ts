import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';
import { LoginComponent } from './login.component';
import { TicketDetailComponent } from './ticket-detail.component';
import { TicketsPageComponent } from './tickets-page.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: '', component: TicketsPageComponent, canActivate: [authGuard] },
  { path: 'tickets/:id', component: TicketDetailComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: '' },
];
