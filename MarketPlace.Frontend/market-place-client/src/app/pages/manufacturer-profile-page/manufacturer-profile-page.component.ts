import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ManufacturerProfile } from '../../data/interfaces/manufacturer-profile';
import { Product } from '../../data/interfaces/product';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { ProductModalComponent } from '../../common-ui/product-modal/product-modal.component';
import { UserService } from '../../data/services/user.service';
import { ProductService } from '../../data/services/product.service';
import { retry } from 'rxjs';
import { ProductFormComponent } from '../../common-ui/product-form/product-form.component';
import { ProductDto } from '../../data/interfaces/product-dto';

const MAX_RETRY_ATTEMPTS = 3;

@Component({
  selector: 'app-manufacturer-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule, ProductCardComponent, ProductModalComponent, ProductFormComponent],
  templateUrl: './manufacturer-profile-page.component.html',
  styleUrls: ['./manufacturer-profile-page.component.scss']
})
export class ManufacturerProfilePageComponent {
  private userService = inject(UserService);
  private productService = inject(ProductService);

  profile: ManufacturerProfile = {
    id: '',
    organization: '',
    logo: null
  };
  
  originalProfile: ManufacturerProfile = { ...this.profile };
  products: Product[] = [];
  selectedProduct: Product | null = null;
  isEditing = false;
  isEditMode = false;
  isCreateMode = false;
  isLoading = true;
  error: string | null = null;

  constructor() {
    this.loadData();
  }

  loadData() {
    this.isLoading = true;
    this.error = null;
    
    this.userService.getManufacturerProfile().pipe(
      retry(MAX_RETRY_ATTEMPTS)
    ).subscribe({
      next: (response) => {
        this.profile = response;
        this.originalProfile = { ...this.profile };

        this.loadProducts();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load profile', err);
        this.error = 'Не удалось загрузить профиль';
        this.isLoading = false;
      }
    });
  }

  loadProducts() {
    this.error = null;
    this.productService.getManufacturerProducts(this.profile.id).pipe(
      retry(MAX_RETRY_ATTEMPTS)
    ).subscribe({
      next: (products) => {
        this.products = products;
        this.error = null;
      },
      error: (err) => {
        console.error('Failed to load products', err);
        this.error = 'Не удалось загрузить товары';
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

  saveProfile() {
    this.isLoading = true;
    this.error = null;

    const updateData = {
      organization: this.profile.organization
    };

    this.userService.updateManufacturerProfile(updateData).subscribe({
      next: () => {
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

  enterEditMode(product: Product) {
    this.closeProductModal();
    this.selectedProduct = product;
    this.isEditMode = true;
    this.isCreateMode = false;
  }

  exitFormMode() {
    this.isEditMode = false;
    this.isCreateMode = false;
    this.selectedProduct = null;
  }

  handleProductSubmit(updatedProduct: ProductDto) {
    this.isLoading = true;
  
  const observable = updatedProduct.id 
    ? this.productService.updateProduct(updatedProduct)
    : this.productService.createProduct(updatedProduct);

  observable.subscribe({
    next: () => {
      this.loadProducts();
      this.exitFormMode();
      this.isLoading = false;
    },
    error: (err) => {
      console.error('Ошибка операции', err);
      this.isLoading = false;
    }
  });
}
  
  deleteProduct(productId: string) {
    this.productService.deleteProduct(productId).subscribe({
      next: () => {
        this.loadProducts(); 
        this.selectedProduct = null; 
      },
      error: (err) => console.error('Ошибка удаления', err)
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
      
      this.userService.updateManufacturerLogo(base64String).subscribe({
        next: () => {
          this.profile.logo = this.userService.getAvatarUrl(base64String);
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Logo upload failed', err);
          this.error = 'Не удалось загрузить логотип';
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

  openProductModal(product: Product) {
    this.selectedProduct = product;
  }

  closeProductModal() {
    this.selectedProduct = null;
  }

  navigateToProductCreation() {
    this.isCreateMode = true;
    this.isEditMode = false;
    this.selectedProduct = null;
    this.closeProductModal();
  }
}
