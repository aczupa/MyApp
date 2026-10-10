import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private loginUrl = 'http://localhost:5115/login';

  token = signal<string | null>(null);
  isLoggedIn = computed(() => this.token() !== null);

  constructor(private http: HttpClient) { }

  login(email: string, password: string) {
    return this.http
      .post<{ accessToken: string }>(this.loginUrl, { email, password })
      .pipe(tap(r => this.token.set(r.accessToken)));
  }

  logout() { this.token.set(null); }
}
