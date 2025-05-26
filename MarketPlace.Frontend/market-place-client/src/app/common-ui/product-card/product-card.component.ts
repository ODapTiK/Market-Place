import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../data/interfaces/product';
import { CommonModule } from '@angular/common';
import { TruncatePipe } from '../../data/pipes/truncate.pipe';
import { ImagePipe } from '../../data/pipes/image.pipe';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [CommonModule, TruncatePipe, ImagePipe],
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss']
})
export class ProductCardComponent {
  @Input() product!: Product;
  @Input() isInCart: boolean = false;
  @Input() isManufacturerView: boolean = false;
  @Output() addToCart = new EventEmitter<void>();
  @Output() removeFromCart = new EventEmitter<void>();
  @Output() openModal = new EventEmitter<void>();
}
