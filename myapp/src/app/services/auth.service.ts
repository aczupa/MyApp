import { Injectable, signal, computed } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class AuthService {
  token = signal<string | null>(null);
  isLoggedIn = computed(() => this.token() !== null);

  setToken(token: string) { this.token.set(token); }
  logout() { this.token.set(null); }
}
