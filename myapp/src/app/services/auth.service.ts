import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap, switchMap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private loginUrl = 'http://localhost:5115/login';
  private registerUrl = 'http://localhost:5115/register';
  private meUrl = 'http://localhost:5115/api/account/me';

  token = signal<string | null>(null);
  isAdmin = signal(false);
  isLoggedIn = computed(() => this.token() !== null);

  constructor(private http: HttpClient, private router: Router) { }

  login(email: string, password: string) {
    return this.http
      .post<{ accessToken: string }>(this.loginUrl, { email, password })
      .pipe(
        tap(r => this.token.set(r.accessToken)),
        switchMap(() => this.http.get<{ isAdmin: boolean }>(this.meUrl)),
        tap(me => this.isAdmin.set(me.isAdmin))
      );
  }

  register(email: string, password: string) {
    return this.http.post(this.registerUrl, { email, password });
  }

  logout() {
    this.token.set(null);
    this.isAdmin.set(false);
    this.router.navigate(['/']);
  }
}
