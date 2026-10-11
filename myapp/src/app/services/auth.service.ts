import { Injectable, signal, computed, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private loginUrl = 'http://localhost:5115/login';
  private readonly key = 'access_token';
  private isBrowser: boolean;

  token = signal<string | null>(null);
  isLoggedIn = computed(() => this.token() !== null);

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(PLATFORM_ID) platformId: Object
  ) {
    this.isBrowser = isPlatformBrowser(platformId);
    if (this.isBrowser) {
      this.token.set(localStorage.getItem(this.key));
    }
  }

  login(email: string, password: string) {
    return this.http
      .post<{ accessToken: string }>(this.loginUrl, { email, password })
      .pipe(tap(r => {
        this.token.set(r.accessToken);
        if (this.isBrowser) localStorage.setItem(this.key, r.accessToken);
      }));
  }

  logout() {
    this.token.set(null);
    if (this.isBrowser) localStorage.removeItem(this.key);
    this.router.navigate(['/']);
  }

  private registerUrl = 'http://localhost:5115/register';

  register(email: string, password: string) {
    return this.http.post(this.registerUrl, { email, password });
  }
}
