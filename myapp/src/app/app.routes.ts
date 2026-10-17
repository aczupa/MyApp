import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Clients } from './clients/clients';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';
import { Register } from './register/register';
import { ClientForm } from './client-form/client-form';

export const routes: Routes = [
  { path: '', component: Login },
  { path: 'clients', component: Clients, canActivate: [authGuard] },
  { path: 'clients/new', component: ClientForm, canActivate: [authGuard, adminGuard] },
  { path: 'register', component: Register },
];
