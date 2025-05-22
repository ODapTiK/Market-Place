import { Component, inject } from '@angular/core';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../data/services/user.service';
import { AuthService } from '../../auth/auth.service';
import { firstValueFrom, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-manufacturer-sidebar',
  imports: [CommonModule, SvgIconComponent, RouterModule],
  templateUrl: './manufacturer-sidebar.component.html',
  styleUrl: './manufacturer-sidebar.component.scss'
})
export class ManufacturerSidebarComponent {
  userService = inject(UserService);
  router = inject(Router);
  authService = inject(AuthService)
  
  unreadCount = 0;
  private notificationSub: Subscription | undefined;
  private notificationReadSub: Subscription | undefined;

  manufacturerProfile = this.userService.manufacturerProfile;

  logout() {
    this.authService.logout();
    this.authService.deleteTokens();
  }

  ngOnInit() {
    firstValueFrom(this.userService.getManufacturerProfile());
    this.userService.getManufacturerUnreadCount().subscribe((count: number) => {
      this.unreadCount = count;
    });
    this.notificationSub = this.userService.notificationReceived.subscribe({
      next: () => {
        this.unreadCount++;
      },
      error: (err) => {
        console.error('Notification error', err);
      }
    });
    this.notificationReadSub = this.userService.onManufacturerNotificationRead().subscribe({
      next: () => {
        this.unreadCount = Math.max(0, this.unreadCount - 1); 
      },
      error: (err) => {
        console.error('Notification read error', err);
      }
    });
  }

  ngOnDestroy() {
    this.notificationSub?.unsubscribe();
    this.notificationReadSub?.unsubscribe(); 
  }
}
