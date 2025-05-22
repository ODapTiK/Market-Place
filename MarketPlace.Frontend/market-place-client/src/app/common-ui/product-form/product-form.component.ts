import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Product } from '../../data/interfaces/product';
import { ProductDto } from '../../data/interfaces/product-dto';
import { ImagePipe } from '../../data/pipes/image.pipe';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ImagePipe],
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss']
})
export class ProductFormComponent {
  editedProduct: ProductDto = {
    id: null,
    name: '',
    description: '',
    category: '',
    type: '',
    image: '',
    price: 0
  };

  isEditMode = false;

  @Input() set product(value: Product | null) {
    console.log(value);
    if (value) {
      this.isEditMode = true;
      const { id, name, description, category, type, image, price } = value;
      this.editedProduct = { id, name, description, category, type, image, price };
      document.body.style.overflow = 'hidden';
    } else {
      this.isEditMode = false;
    }
  }
  @Output() submit = new EventEmitter<ProductDto>();
  @Output() cancel = new EventEmitter<void>();

  ngOnInit() {
    document.body.style.overflow = 'hidden';
  }

  ngOnDestroy() {
    document.body.style.overflow = '';
  }

  onImageChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => {
        const base64String = (e.target?.result as string).split(',')[1];
        this.editedProduct.image = base64String;
      };
      reader.readAsDataURL(file);
    }
  }

  triggerFileInput() {
    document.getElementById('fileInput')?.click();
  }

  onSubmit() {
    this.submit.emit(this.editedProduct);
  }

  onCancel() {
    const modal = document.querySelector('.modal-content') as HTMLElement;
    if (modal) {
      modal.style.animation = 'slideUp 0.3s ease reverse';
      setTimeout(() => {
        this.cancel.emit();
      }, 250);
    } else {
      this.cancel.emit();
    }
  }

  isFormValid(): boolean {
    return this.editedProduct.price >= 0;
  }
}