import { Component, inject } from '@angular/core';
import { AdminProfile } from '../../data/interfaces/admin-profile';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../data/services/order.service';
import { Order } from '../../data/interfaces/order';
import { DatePipe } from '@angular/common';
import { TruncatePipe } from '../../data/pipes/truncate.pipe';
import { ImagePipe } from '../../data/pipes/image.pipe';
import { UserService } from '../../data/services/user.service';
import { retry } from 'rxjs';

const MAX_RETRY_ATTEMPTS = 3;

@Component({
  selector: 'app-admin-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, TruncatePipe, ImagePipe],
  templateUrl: './admin-profile-page.component.html',
  styleUrls: ['./admin-profile-page.component.scss']
})
export class AdminProfilePageComponent {
  private adminService = inject(UserService);
  private orderService = inject(OrderService);

  profile: AdminProfile = {
    id: '',
    name: '',
    surname: '',
    logo: null,
    adminControlOrdersId: []
  };
  
  originalProfile: AdminProfile = { ...this.profile };
  isEditing = false;
  selectedOrder: Order | null = null;
  orders: Order[] = [];
  isLoading = true;
  error: string | null = null;

  constructor() {
    this.loadAdminProfile();
  }

  loadAdminProfile() {
    this.isLoading = true;
    this.error = null;
    
    this.adminService.getAdminProfile().pipe(
      retry(MAX_RETRY_ATTEMPTS)
    ).subscribe({
      next: (response) => {
        this.profile = response;
        this.originalProfile = { ...this.profile };
        this.orderService.getOrdersByIdList({orderIds: this.profile.adminControlOrdersId}).pipe(
          retry(MAX_RETRY_ATTEMPTS)
        ).subscribe({
          next: (orders) => {
            this.orders = orders.map(order => ({
              ...order,
              orderDateTime: new Date(order.orderDateTime) 
            }));
          },
          error: (err) => {
            console.error('Failed to load orders', err);
            this.error = 'Не удалось загрузить заказы';
          }
        });
        this.isLoading = false;
        this.error = null;
      },
      error: (err) => {
        console.error('Failed to load profile', err);
        this.error = 'Не удалось загрузить профиль';
        this.isLoading = false;
      }
    });
  }

  startEditing() {
    this.isEditing = true;
  }

  cancelEditing() {
    this.profile = { ...this.originalProfile };
    this.isEditing = false;
  }

  markOrderAsReady(orderId: string) {
    if (confirm('Вы уверены, что хотите отметить заказ как готовый к выдаче?')) {
      this.orderService.setOrderStatusReady(orderId).subscribe({
        next: () => {
          const order = this.orders.find(o => o.id === orderId);
          if (order) {
            order.status = 'completed';
          }
          
          if (this.selectedOrder && this.selectedOrder.id === orderId) {
            this.selectedOrder.status = 'completed';
          }
        },
        error: (err) => {
          console.error('Ошибка при изменении статуса заказа:', err);
        }
      });
    }
  }

  saveProfile() {
    this.isLoading = true;
    this.error = null;

    const updateData = {
      Name: this.profile.name,
      Surname: this.profile.surname
    };

    this.adminService.updateAdminProfile(updateData).subscribe({
      next: () => {
        this.profile = {
          ...this.profile,
          name: this.profile.name,
          surname: this.profile.surname
        };
        this.originalProfile = { ...this.profile };
        this.isEditing = false;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to update profile', err);
        this.error = 'Не удалось сохранить изменения';
        if (err.error?.message) {
          this.error += `: ${err.error.message}`;
        }
        this.isLoading = false;
      }
    });
  }

  onAvatarChange(event: Event) {
    const input = event.target as HTMLInputElement;
  
    if (!input.files?.length) {
      return;
    }

    const file = input.files[0];
    
    if (!file.type.match('image.*')) {
      this.error = 'Пожалуйста, выберите файл изображения';
      return;
    }

    this.isLoading = true;
    this.error = null;

    const reader = new FileReader();
    
    reader.onload = (e) => {
      const base64String = (e.target?.result as string).split(',')[1];
      
      this.adminService.updateAdminLogo(base64String).subscribe({
        next: () => {
          this.profile.logo = this.adminService.getAvatarUrl(base64String);
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Avatar upload failed', err);
          this.error = 'Не удалось загрузить аватар';
          if (err.error?.message) {
            this.error += `: ${err.error.message}`;
          }
          this.isLoading = false;
        }
      });
    };
    
    reader.onerror = () => {
      this.error = 'Ошибка при чтении файла';
      this.isLoading = false;
    };
    
    reader.readAsDataURL(file);
  }

  openOrderModal(order: Order) {
    this.selectedOrder = order;
  }

  closeOrderModal() {
    this.selectedOrder = null;
  }
}