import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: '../login/login.scss',
})
export class Register {
  email = '';
  password = '';
  confirm = '';
  error = signal('');

  constructor(private router: Router, private auth: AuthService) { }

  register() {
    if (this.password !== this.confirm) {
      this.error.set('Les mots de passe ne correspondent pas.');
      return;
    }

    this.auth.register(this.email, this.password).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => {
        const errors = err.error?.errors ?? {};
        const duplicate = 'DuplicateUserName' in errors || 'DuplicateEmail' in errors;
        this.error.set(
          duplicate
            ? 'Cette adresse e-mail est déjà utilisée.'
            : 'Le mot de passe doit contenir au moins 6 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial.'
        );
      },
    });
  }
}
