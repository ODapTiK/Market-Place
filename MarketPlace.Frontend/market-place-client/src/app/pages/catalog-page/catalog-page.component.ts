import { Component, inject, OnInit } from '@angular/core';
import { ProductService } from '../../data/services/product.service';
import { Product } from '../../data/interfaces/product';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { ProductModalComponent } from '../../common-ui/product-modal/product-modal.component';
import { OrderService } from '../../data/services/order.service';
import { Cart } from '../../data/interfaces/cart';
import { ErrorHandlerService } from '../../data/services/error-handler.service';

@Component({
  selector: 'app-catalog-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ProductCardComponent, ProductModalComponent],
  templateUrl: './catalog-page.component.html',
  styleUrls: ['./catalog-page.component.scss']
})
export class CatalogPageComponent {
  private productService = inject(ProductService);
  private orderService = inject(OrderService);
  private errorHandler = inject(ErrorHandlerService);

  products: Product[] = [];
  cart: Cart | null = null;
  userId: string = '';
  filteredProducts: Product[] = [];
  selectedProduct: Product | null = null;
  isLoading = true;
  error: string | null = null;
  
  searchQuery = '';
  selectedCategory = '';
  selectedType = '';
  sortOption = 'price-asc';
  
  categories: string[] = [];
  types: string[] = [];

  constructor() {
    this.loadProducts();
    this.loadCart();
  }

  loadProducts() {
    this.isLoading = true;
    this.error = null;
    
    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.filteredProducts = [...products];
        
        this.categories = Array.from(new Set(products.map(p => p.category).filter(Boolean))) as string[];
        this.types = Array.from(new Set(products.map(p => p.type).filter(Boolean))) as string[];
        
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to load products")
        this.isLoading = false;
      }
    });
  }

  loadCart() {
    this.isLoading = true;
    this.error = null;

    this.orderService.loadUserCart().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.userId = cart.userId;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to load cart")
        this.isLoading = false;
      }
    }); 
  }

  applyFilters() {
    let result = [...this.products];
    
    if (this.searchQuery) {
      const query = this.searchQuery.toLowerCase();
      result = result.filter(p => 
        p.name?.toLowerCase().includes(query) || 
        p.description?.toLowerCase().includes(query)
      );
    }
    
    if (this.selectedCategory) {
      result = result.filter(p => p.category === this.selectedCategory);
    }
    
    if (this.selectedType) {
      result = result.filter(p => p.type === this.selectedType);
    }
    
    this.applySorting(result);
  }

  applySorting(products = this.filteredProducts) {
    let sorted = [...products];
    
    switch (this.sortOption) {
      case 'price-asc':
        sorted.sort((a, b) => (a.price || 0) - (b.price || 0));
        break;
      case 'price-desc':
        sorted.sort((a, b) => (b.price || 0) - (a.price || 0));
        break;
      case 'rating-desc':
        sorted.sort((a, b) => (b.raiting || 0) - (a.raiting || 0));
        break;
      case 'newest':
        sorted.sort((a, b) => new Date(b.creationDateTime || 0).getTime() - new Date(a.creationDateTime || 0).getTime());
        break;
    }
    
    this.filteredProducts = sorted;
  }

  resetFilters() {
    this.searchQuery = '';
    this.selectedCategory = '';
    this.selectedType = '';
    this.sortOption = 'price-asc';
    this.applyFilters();
  }

  openProductModal(product: Product) {
    this.selectedProduct = product;
    this.productService.getProduct(product.id).subscribe({
      next: () => {
        this.loadProducts();
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to load product")
        this.isLoading = false;
      }
    });
  }

  closeProductModal() {
    this.selectedProduct = null;
  }

  isInCart(productId: string): boolean {
    if (!productId || !this.cart?.products) {
      return false;
    }
    return this.cart.products.some(x => x === productId);
  }

  addToCart(product: Product) {
    this.isLoading = true;
    this.error = null;

    this.productService.addProductToUserCart(product.id).subscribe({
      next: () => {
        this.orderService.loadUserCart().subscribe({
          next: (cart) => {
            this.cart = cart;
            this.isLoading = false;
          },
          error: (err) => {
            this.errorHandler.handleError(err, "Unable to load cart")
            this.isLoading = false;
          }
        })
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to add product to cart")
        this.isLoading = false;
      }
    });
  }

  removeFromCart(productId: string) {
    this.isLoading = true;
    this.error = null;
    
    this.productService.removeProductFromUserCart(productId).subscribe({
      next: () => {
        this.orderService.loadUserCart().subscribe({
          next: (cart) => {
            this.cart = cart;
            this.isLoading = false;
          },
          error: (err) => {
            this.errorHandler.handleError(err, "Unable to load cart")
            this.isLoading = false;
          }
        })
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to remove product from cart")
        this.isLoading = false;
      }
    });
  }

  onDeleteReview(productId: string, reviewId: string) {
    this.productService.deleteReview(productId, reviewId).subscribe({
      next: () => {
        this.closeProductModal();
        this.loadProducts();
      },
      error: (err) => {
        this.errorHandler.handleError(err, "Unable to remove product review")
      }
    });
  }
}
