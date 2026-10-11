import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-logout-button',
  template: `<button (click)="auth.logout()">Se déconnecter</button>`,
})
export class LogoutButton {
  constructor(protected auth: AuthService) { }
}
