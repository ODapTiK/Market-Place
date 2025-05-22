import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../data/services/product.service';
import { OrderService } from '../../data/services/order.service';
import { ImagePipe } from '../../data/pipes/image.pipe';
import { TruncatePipe } from '../../data/pipes/truncate.pipe';
import { UserService } from '../../data/services/user.service';
import { AdminProfile } from '../../data/interfaces/admin-profile';
import { Cart } from '../../data/interfaces/cart';
import { Product } from '../../data/interfaces/product';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ErrorHandlerService } from '../../data/services/error-handler.service';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ImagePipe, TruncatePipe],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss']
})
export class CartPageComponent {
  private userService = inject(UserService);
  private productService = inject(ProductService);
  private orderService = inject(OrderService);
  private router = inject(Router);
  private errorHandler = inject(ErrorHandlerService);

  admins: AdminProfile[] = [];
  cart: Cart | null = null;
  products: Product[] = [];
  selectedProducts: { [key: string]: boolean } = {};
  quantities: { [key: string]: number } = {};
  selectedAdminId: string | null = null;
  isLoading = true;
  error: string | null = null;
  isCreatingOrder = false;

  constructor() {
    this.loadPageData();
  }

  loadPageData() {
    this.loadControlAdmins();
    this.loadCartProducts();
  }

  loadControlAdmins() {
    this.isLoading = true;
    this.error = null;

    this.userService.getAdmins().subscribe({
      next: (admins) => {
        this.admins = admins;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to control admins")
        this.isLoading = false;
      }
    })
  }

  loadCartProducts() {
    this.isLoading = true;
    this.error = null;

    this.orderService.loadUserCart().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.productService.getProductsByIdList({productIds: this.cart.products}).subscribe({
          next: (products) => {
            this.products = products;

            this.products.forEach(product => {
              this.selectedProducts[product.id] = true;
              this.quantities[product.id] = 1;
            });
          },
          error: (err) => {
            this.errorHandler.handleError(err, "Unable to products")
            this.isLoading = false;
          }
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to load cart")
        this.isLoading = false;
      }
    })
  }

  startOrderCreation() {
    if (this.getSelectedProducts().length === 0) {
      this.error = 'Выберите хотя бы один товар';
      return;
    }
    this.isCreatingOrder = true;
  }

  cancelOrderCreation() {
    this.isCreatingOrder = false;
    this.selectedAdminId = null;
  }

  getSelectedProducts() {
    return this.products.filter(p => this.selectedProducts[p.id]);
  }

  getTotalPrice() {
    return this.getSelectedProducts().reduce((total, product) => {
      return total + (product.price * (this.quantities[product.id] || 1));
    }, 0);
  }

  getProductName(product: Product): string {
    return product.name || 'Без названия';
  }

  getProductDescription(product: Product): string {
    return product.description || 'Описание отсутствует';
  }

  getProductCategory(product: Product): string {
    return product.category || 'Без категории';
  }

  getProductType(product: Product): string {
    return product.type || 'Без типа';
  }

  createOrder() {
    if (!this.selectedAdminId) {
      this.error = 'Выберите администратора';
      return;
    }
  
    const selectedProducts = this.getSelectedProducts();
    if (selectedProducts.length === 0) {
      this.error = 'Выберите хотя бы один товар';
      return;
    }
  
    const orderItems = selectedProducts.map(product => ({
      productId: product.id,
      numberOfUnits: this.quantities[product.id] || 1
    }));
  
    this.isLoading = true;
    this.orderService.createOrder({
      controlAdminId: this.selectedAdminId,
      points: orderItems
    }).subscribe({
      next: () => {
        const productIdsToRemove = selectedProducts.map(p => p.id);
        
        this.products = this.products.filter(p => !productIdsToRemove.includes(p.id));
        
        productIdsToRemove.forEach(id => {
          delete this.selectedProducts[id];
          delete this.quantities[id];
        });
  
        const removeRequests = productIdsToRemove.map(id => 
          this.productService.removeProductFromUserCart(id)
        );
  
        forkJoin(removeRequests).subscribe({
          next: () => {
            this.isCreatingOrder = false;
            this.selectedAdminId = null;
            this.isLoading = false;
          },
          error: (err) => {
            this.errorHandler.handleError(err, "Unable to create order")
            this.isCreatingOrder = false;
            this.selectedAdminId = null;
            this.isLoading = false;
          }
        });
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to create order")
        this.isLoading = false;
      }
    });
  }

  removeFromCart(productId: string) {
    this.isLoading = true;
    this.error = null;
    
    this.productService.removeProductFromUserCart(productId).subscribe({
      next: () => {
        this.loadCartProducts();
        this.isLoading = false;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to remove product from cart")
        this.isLoading = false;
      }
    });
  }

  navigateToCatalog() {
    this.router.navigate(['/user/catalog']);
  }
}