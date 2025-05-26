import { Component, inject } from '@angular/core';
import { UserService } from '../../data/services/user.service';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { firstValueFrom, Subscription } from 'rxjs';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-sidebar',
  imports: [CommonModule, SvgIconComponent, RouterModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss'
})
export class AdminSidebarComponent {
  userService = inject(UserService);
  router = inject(Router);
  authService = inject(AuthService)
  
  unreadCount = 0;
  private notificationSub: Subscription | undefined;
  private notificationReadSub: Subscription | undefined;

  adminProfile = this.userService.adminProfile;

  logout() {
    this.authService.deleteTokens();
    this.authService.logout();
  }

  ngOnInit() {
    firstValueFrom(this.userService.getAdminProfile());
    this.userService.getAdminUnreadCount().subscribe((count: number) => {
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
    this.notificationReadSub = this.userService.onAdminNotificationRead().subscribe({
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
