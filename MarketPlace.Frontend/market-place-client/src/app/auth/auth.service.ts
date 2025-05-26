import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Tokens } from './tokens';
import { catchError, tap, throwError } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient = inject(HttpClient);
  router = inject(Router);
  cookieService = inject(CookieService);

  baseApiAuthUrl = 'https://localhost:6011/api';

  get isAuth(): boolean {
    return !!this.cookieService.get('access_token');
  }
  get userRole() {
    if (!this.isAuth) {
      this.logout();
      return null;
    }
    return this.cookieService.get('role');
  }

  login(payload: {email: string, password: string}) {
    return this.httpClient.post<Tokens>(`${this.baseApiAuthUrl}/user/auth`, payload)
      .pipe(
        tap(tokens => {
          console.log("login");
          this.writeTokens(tokens);
        })
      );
  }

  refreshAccessToken() {
    return this.httpClient.put<Tokens>(`${this.baseApiAuthUrl}/token`, 
      {
        accessToken: this.cookieService.get('access_token'),
        refreshToken: this.cookieService.get('refresh_token'),
      }).pipe(
        tap(tokens => {
          console.log("refresh")
          this.writeTokens(tokens);
        }),
        catchError(error => {
          this.logout();
          return throwError(error);
        })
      )
  }

  logout() {
    this.deleteTokens();

    this.router.navigate(['login']);

    console.log(this.cookieService.getAll());
  }

  deleteTokens(){
    const path = '/';
    this.cookieService.delete('access_token', path);
    this.cookieService.delete('refresh_token', path);
    this.cookieService.delete('role', path);
  }

  writeTokens(tokens: Tokens) {
    console.log("set cookie");
    this.deleteTokens();

    const path = '/';
    this.cookieService.set('access_token', tokens.accessToken, {path: path});
    this.cookieService.set('refresh_token', tokens.refreshToken, {path: path});
    this.cookieService.set('role', tokens.role, {path: path});
  }
}
