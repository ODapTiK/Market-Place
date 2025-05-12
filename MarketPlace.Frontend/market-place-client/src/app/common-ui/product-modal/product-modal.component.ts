import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Product } from '../../data/interfaces/product';
import { ReviewCardComponent } from '../review-card/review-card.component';
import { ImagePipe } from '../../data/pipes/image.pipe';

@Component({
  selector: 'app-product-modal',
  standalone: true,
  imports: [CommonModule, ReviewCardComponent, ImagePipe],
  templateUrl: './product-modal.component.html',
  styleUrls: ['./product-modal.component.scss']
})
export class ProductModalComponent {
  @Input() product!: Product;
  @Input() userId: string | null = null;
  @Input() isInCart: boolean = false;
  @Input() isManufacturerView: boolean = false;
  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Product>();
  @Output() delete = new EventEmitter<string>();
  @Output() addToCart = new EventEmitter<void>();
  @Output() removeFromCart = new EventEmitter<void>();
  @Output() deleteReview = new EventEmitter<{productId: string, reviewId: string}>();

  getViewsCount(period: string): number {
    const now = new Date();
    let dateFilter: Date;
    
    switch (period) {
      case 'hour':
        dateFilter = new Date(now.getTime() - 60 * 60 * 1000);
        break;
      case 'day':
        dateFilter = new Date(now.getTime() - 24 * 60 * 60 * 1000);
        break;
      case 'week':
        dateFilter = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
        break;
      default:
        return 0;
    }
    
    return this.product.viewAt
      ? this.product.viewAt.filter(view => new Date(view) >= dateFilter).length
      : 0;
  }

  onDelete() {
    if (confirm('Вы уверены, что хотите удалить этот товар?')) {
      this.delete.emit(this.product.id);
    }
  }

  handleDeleteReview(reviewId: string){
    this.deleteReview.emit({
      productId: this.product.id,
      reviewId: reviewId
    });
  }

  onEdit() {
    this.edit.emit(this.product); 
  }
}