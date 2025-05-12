import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  httpClient = inject(HttpClient);

  baseApiAuthServiceUrl = 'https://localhost:6011/api';

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
