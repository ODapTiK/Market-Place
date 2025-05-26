import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { UserService } from '../../data/services/user.service';
import { Notification } from '../../data/interfaces/notification';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-admin-notification-page',
  imports: [CommonModule, DatePipe],
  templateUrl: './admin-notification-page.component.html',
  styleUrl: './admin-notification-page.component.scss'
})
export class AdminNotificationPageComponent {
  baseApiUrl = "https://localhost:6012"

  notifications: Notification[] = [];
  private hubConnection!: HubConnection;
  private userService = inject(UserService);
  userId: string = '';

  ngOnInit(): void {
    this.loadAdmin();
    this.initSignalR();
  }

  ngOnDestroy(): void {
    this.hubConnection?.stop();
  }

  loadAdmin(){
    this.userService.getAdminProfile().subscribe({
      next: (response) => {
        this.userId = response.id; 
        this.notifications = response.adminNotifications;
      },
      error: (err) => {
        console.error('Failed to load profile', err);
      }
    });
  }

  private initSignalR(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${this.baseApiUrl}/notificationHub`, {withCredentials: false})
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        console.log('SignalR Connected');
        this.joinNotificationGroup();
      })
      .catch(err => console.log('Error while starting connection: ' + err));

      this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
        console.log("message get");
        this.notifications.unshift(notification);
        this.userService.notifyNewNotification(notification); 
      });
  }

  private joinNotificationGroup(): void {
    this.hubConnection.invoke('JoinGroup', this.userId)
      .catch(err => console.error('Error joining group:', err));
  }

  markAsRead(notificationId: string): void {
    this.userService.readAdminNotification(notificationId).subscribe({
      next: () => {
        const notification = this.notifications.find(n => n.id === notificationId);
        if (notification && !notification.isRead) {
          notification.isRead = true;
          this.userService.notifyAdminNotificationRead(); 
        }
      },
      error: (err) => {
        console.error('Failed to mark notification as read', err);
      }
    });
  }
}
