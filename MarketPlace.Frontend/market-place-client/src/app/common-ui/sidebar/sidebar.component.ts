import { Component, inject } from '@angular/core';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { UserService } from '../../data/services/user.service';
import { firstValueFrom, Subscription } from 'rxjs';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sidebar',
  imports: [CommonModule, SvgIconComponent, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  userService = inject(UserService);
  router = inject(Router);
  authService = inject(AuthService);

  unreadCount = 0;
  private notificationSub: Subscription | undefined;
  private notificationReadSub: Subscription | undefined;
  
  userProfile = this.userService.userProfile;

  logout() {
    this.authService.deleteTokens();
    this.authService.logout();
  }

  ngOnInit() {
    firstValueFrom(this.userService.getUserProfile());
    this.userService.getUserUnreadCount().subscribe((count: number) => {
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
    this.notificationReadSub = this.userService.onUserNotificationRead().subscribe({
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
