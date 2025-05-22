import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Review } from '../../data/interfaces/review';

@Component({
  selector: 'app-review-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-card.component.html',
  styleUrls: ['./review-card.component.scss']
})
export class ReviewCardComponent {
  @Input() review!: Review;
  @Input() currentUserId: string | null = null;
  @Output() onDelete = new EventEmitter<string>(); 

  get isCurrentUserAuthor(): boolean {
    return this.currentUserId === this.review.userId;
  }

  deleteReview(): void {
    if (confirm('Вы уверены, что хотите удалить этот отзыв?')) {
      this.onDelete.emit(this.review.id);
    }
  }
}
