import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Review } from '../../data/interfaces/review';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-review-form',
  imports: [CommonModule, FormsModule],
  templateUrl: './review-form.component.html',
  styleUrl: './review-form.component.scss'
})
export class ReviewFormComponent {
  @Input() productName: string = '';
  @Output() onSubmit = new EventEmitter<Review>();
  @Output() onCancel = new EventEmitter<void>();

  review: Review = {
    id: '',
    userId: '',
    description: '',
    raiting: 0 
  };

  setRating(rating: number): void {
    this.review.raiting = rating;
  }

  submitReview(): void {
    if (this.review.description && this.review.raiting > 0) {
      this.onSubmit.emit(this.review);
    }
  }
}
