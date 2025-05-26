import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlerService {
constructor(private snackBar: MatSnackBar) {}

  handleError(error: any, defaultMessage: string = 'Произошла ошибка'): string {
    const errorMessage = this.getErrorMessage(error, defaultMessage);
    console.error('Ошибка:', error);
    this.showSnackbar(errorMessage);
    return errorMessage;
  }

  private showSnackbar(message: string): void {
    this.snackBar.open(message, 'Закрыть', {
      duration: 5000, 
      panelClass: ['error-snackbar']
    });
  }

  private getErrorMessage(error: any, defaultMessage: string): string {
    if (error?.error?.message) return error.error.message;
    if (error?.message) return error.message;
    if (typeof error === 'string') return error;
    return defaultMessage;
  }
}
