import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Clients } from './clients/clients';
import { authGuard } from './guards/auth.guard';
import { adminGuard } from './guards/admin.guard';
import { Register } from './register/register';
import { ClientForm } from './client-form/client-form'; import { Coworkers } from './coworkers/coworkers';
import { CoworkerDetail } from './coworker-detail/coworker-detail';



export const routes: Routes = [
  { path: '', component: Login },
  { path: 'clients', component: Clients, canActivate: [authGuard] },
  { path: 'clients/new', component: ClientForm, canActivate: [authGuard, adminGuard] },
  { path: 'clients/:id/edit', component: ClientForm, canActivate: [authGuard, adminGuard] },
  {
    path: 'clients/:id',
    loadComponent: () =>
      import('./client-detail/client-detail')
        .then(m => m.ClientDetail)
  },
  { path: 'register', component: Register },
  { path: 'coworkers', component: Coworkers, canActivate: [authGuard] },
  { path: 'coworkers/:id', component: CoworkerDetail, canActivate: [authGuard] },
];
