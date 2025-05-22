import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { UserProfile } from '../interfaces/user-profile';
import { catchError, tap, throwError } from 'rxjs';
import { ManufacturerProfile } from '../interfaces/manufacturer-profile';
import { AdminProfile } from '../interfaces/admin-profile';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  httpClient = inject(HttpClient);

  baseApiUserServiceUrl = environment.apiUrls.baseApiUrl;

  userProfile = signal<UserProfile | null>(null);
  manufacturerProfile = signal<ManufacturerProfile | null>(null);
  adminProfile = signal<AdminProfile | null>(null);

  getAdmins() {
    return this.httpClient.get<AdminProfile[]>(`${this.baseApiUserServiceUrl}/admins`);
  }

  getAdminProfile() {
    return this.httpClient.get<AdminProfile>(`${this.baseApiUserServiceUrl}/admin`)
    .pipe(
      tap(response => response.logo = this.getAvatarUrl(response.logo!)),
      tap(response => this.adminProfile.set(response))
    );
  }

  getUserProfile() {
    return this.httpClient.get<UserProfile>(`${this.baseApiUserServiceUrl}/user`)
      .pipe(
        tap(response => response.logo = this.getAvatarUrl(response.logo!)),
        tap(response => this.userProfile.set(response))
      )
  }

  updateUserProfile(userData: {
    Name?: string;
    Surname?: string;
    BirthDate?: string;
  }) {
    return this.httpClient.put(`${this.baseApiUserServiceUrl}/user`, userData);
  }

  updateAdminProfile(adminData: {
    Name?: string;
    Surname?: string;
  }) {
    return this.httpClient.put(`${this.baseApiUserServiceUrl}/admin`, adminData);
  }

  updateUserLogo(base64Logo: string) {
    return this.httpClient.put(`${this.baseApiUserServiceUrl}/user/logo`, 
      { base64Logo },
      { headers: { 'Content-Type': 'application/json' } }
    )
    .pipe(
      tap(() => {
        const currentProfile = this.userProfile();
        if (currentProfile) {
          this.userProfile.set({
            ...currentProfile,
            logo: this.getAvatarUrl(base64Logo), 
          });
        }
      }),
      catchError(error => {
        console.error('Error updating logo:', error);
        return throwError(() => error);
      })
    );
  }

  updateAdminLogo(base64Logo: string) {
    return this.httpClient.put(`${this.baseApiUserServiceUrl}/admin/logo`, 
      { base64Logo },
      { headers: { 'Content-Type': 'application/json' } }
    )
    .pipe(
      tap(() => {
        const currentProfile = this.adminProfile();
        if (currentProfile) {
          this.adminProfile.set({
            ...currentProfile,
            logo: this.getAvatarUrl(base64Logo), 
          });
        }
      }),
      catchError(error => {
        console.error('Error updating logo:', error);
        return throwError(() => error);
      })
    );
  }


  getManufacturerProfile() {
    return this.httpClient.get<ManufacturerProfile>(`${this.baseApiUserServiceUrl}/manufacturer`)
      .pipe(
        tap(response => response.logo = this.getAvatarUrl(response.logo!)),
        tap(response => this.manufacturerProfile.set(response))
      );
  }

  updateManufacturerProfile(manufacturerData: { 
    organization: string 
  }) {
    return this.httpClient.put(`${this.baseApiUserServiceUrl}/manufacturer`, manufacturerData);
  }

  updateManufacturerLogo(base64Logo: string) {
    return this.httpClient.put(`${this.baseApiUserServiceUrl}/manufacturer/logo`, 
      { base64Logo },
      { headers: { 'Content-Type': 'application/json' } }
    )
    .pipe(
      tap(() => {
        const currentProfile = this.manufacturerProfile();
        if (currentProfile) {
          this.manufacturerProfile.set({
            ...currentProfile,
            logo: this.getAvatarUrl(base64Logo), 
          });
        }
      }),
      catchError(error => {
        console.error('Error updating logo:', error);
        return throwError(() => error);
      })
    );
  }

  getAvatarUrl(logo: string | null): string {
    if (!logo) {
      return '/assets/default-images/user-logo.jpg';
    }
    
    if (logo.startsWith('data:image')) {
      return logo;
    }
    
    const imageType = this.detectImageType(logo);

    return `data:image/${imageType};base64,${logo}`;
  }

  private detectImageType(base64: string): string {
    if (base64.startsWith('/9j') || base64.startsWith('/9j/4')) {
      return 'jpeg';
    }
    else if (base64.startsWith('iVBORw0KGgo')) {
      return 'png';
    }
    else if (base64.startsWith('R0lGODdh') || base64.startsWith('R0lGODlh')) {
      return 'gif';
    }
    else if (base64.startsWith('UklGR')) {
      return 'webp';
    }

    return 'jpeg';
  }
}
