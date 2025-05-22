import { Component, inject } from '@angular/core';
import { UserService } from '../../data/services/user.service';
import { UserProfile } from '../../data/interfaces/user-profile';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../data/services/order.service';
import { Order, OrderPoint } from '../../data/interfaces/order';
import { DatePipe } from '@angular/common';
import { TruncatePipe } from '../../data/pipes/truncate.pipe';
import { retry } from 'rxjs';
import { ImagePipe } from '../../data/pipes/image.pipe';
import { ProductService } from '../../data/services/product.service';
import { Review } from '../../data/interfaces/review';
import { ReviewFormComponent } from '../../common-ui/review-form/review-form.component';
import { ErrorHandlerService } from '../../data/services/error-handler.service';
import { Router, RouterModule } from '@angular/router';

const MAX_RETRY_ATTEMPTS = 3;

@Component({
  selector: 'app-user-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe, TruncatePipe, ImagePipe, ReviewFormComponent, RouterModule],
  templateUrl: './user-profile-page.component.html',
  styleUrls: ['./user-profile-page.component.scss']
})
export class UserProfilePageComponent {
  private userService = inject(UserService);
  private orderService = inject(OrderService);
  private productService = inject(ProductService);
  private errorHandler = inject(ErrorHandlerService);
  private router = inject(Router);

  profile: UserProfile = {
    id: '',
    name: '',
    surname: '',
    logo: null,
    birthDate: '',
    userOrdersId: [],
    userNotifications: []
  };
  
  showReviewForm: { [key: string]: boolean } = {};
  originalProfile: UserProfile = { ...this.profile };
  isEditing = false;
  selectedOrder: Order | null = null;
  orders: Order[] = [];
  isLoading = true;
  error: string | null = null;

  constructor() {
    this.loadUserProfile();
    this.loadOrders();
  }

  loadUserProfile() {
    this.isLoading = true;
    this.error = null;
    
    this.userService.getUserProfile().pipe(
      retry(MAX_RETRY_ATTEMPTS)
    ).subscribe({
      next: (response) => {
        this.profile = {
          ...response,
          birthDate: this.formatDateForInput(response.birthDate)
        };
        this.originalProfile = { ...this.profile };
        this.isLoading = false;
        this.error = null;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to load profile");
        this.isLoading = false;
      }
    });
  }

  private formatDateForInput(dateString: string | Date): string {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    
    return `${year}-${month}-${day}`;
  }

  loadOrders() {
    this.error = null;
    this.orderService.getUserOrders().pipe(
      retry(MAX_RETRY_ATTEMPTS)
    ).subscribe({
      next: (orders) => {
        this.orders = orders.map(order => ({
          ...order,
          orderDateTime: new Date(order.orderDateTime) 
        }));
        this.error = null;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to load orders");
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

  cancelOrder(orderId: string) {
    if (confirm('Вы уверены, что хотите отменить этот заказ?')) {
      this.orderService.cancelOrder(orderId).subscribe({
        next: () => {
          const order = this.orders.find(o => o.id === orderId);
          if (order) {
            order.status = 'Cancelled';
          }
        },
        error: (err) => {
          this.errorHandler.handleError(err, "Cancellation order error");
        }
      });
    }
  }

  saveProfile() {
      this.isLoading = true;
      this.error = null;

      const updateData = {
        Name: this.profile.name,
        Surname: this.profile.surname,
        BirthDate: this.profile.birthDate 
      };

      this.userService.updateUserProfile(updateData).subscribe({
        next: () => {
          this.profile = {
            ...this.profile,
            name: this.profile.name,
            surname: this.profile.surname,
            birthDate: this.formatDateForInput(this.profile.birthDate)
          };
          this.originalProfile = { ...this.profile };
          this.isEditing = false;
          this.isLoading = false;
        },
        error: (err) => {
          this.errorHandler.handleError(err, "Unable to update profile");
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
      
      this.userService.updateUserLogo(base64String).subscribe({
        next: () => {
          this.profile.logo = this.userService.getAvatarUrl(base64String);
          this.isLoading = false;
        },
        error: (err) => {
          this.errorHandler.handleError(err, "Unable to update logo");
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

  canLeaveReview(order: Order): boolean {
    return order.status.toLowerCase() === 'completed';
  }

  openReviewForm(orderPoint: OrderPoint): void {
    this.showReviewForm = {}; 
    this.showReviewForm[orderPoint.id] = true;
  }

  closeReviewForm(orderPointId: string): void {
    this.showReviewForm[orderPointId] = false;
  }

  submitReview(review: Review, orderPoint: OrderPoint): void {
    const reviewPayload = {
      raiting: review.raiting,
      description: review.description
    };

    this.productService.createReview(orderPoint.productId, reviewPayload).subscribe({
      next: () => {
        this.closeReviewForm(orderPoint.id);
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to create review");
      }
    });
  }

  navigateToCatalog() {
    this.router.navigate(['/user/catalog']);
  }
}