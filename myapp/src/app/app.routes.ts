import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Clients } from './clients/clients';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'clients', component: Clients, canActivate: [authGuard] },
];
