import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  email = '';
  password = '';
  error = signal('');

  constructor(private router: Router, private auth: AuthService) { }

  login() {
    this.auth.login(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/clients']),
      error: () => this.error.set('Adresse e-mail ou mot de passe incorrect.'),
    });
  }
}
