import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient = inject(HttpClient);

  baseApiAuthServiceUrl = environment.apiUrls.auth;

  createUser(userData: {
    Email: string;
    Password: string;
    Role: 'User' | 'Manufacturer';
    Name?: string;
    Surname?: string;
    BirthDate?: string;
    Organization?: string;
  }): Observable<any> {
    return this.httpClient.post(
      `${this.baseApiAuthServiceUrl}/user`, 
      userData
    );
  }
}
