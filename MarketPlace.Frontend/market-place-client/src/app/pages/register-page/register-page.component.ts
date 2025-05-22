import { Component, ElementRef, HostListener, inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../data/services/auth.service';
import { UserRegistrationForm } from '../../data/interfaces/user-registration-form';
import { ManufacturerRegistrationForm } from '../../data/interfaces/manufacturer-registration-form';
import { Router } from '@angular/router';
import { ErrorHandlerService } from '../../data/services/error-handler.service';

@Component({
  selector: 'app-registration',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.scss']
})
export class RegisterPageComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private errorHandler = inject(ErrorHandlerService);

  isProducerForm = false;
  switching = false;
  
  constructor(private elRef: ElementRef) {}

  @HostListener('document:mousemove', ['$event'])
  onMouseMove(event: MouseEvent) {
    const buttons = this.elRef.nativeElement.querySelectorAll('.button');
    buttons.forEach((button: HTMLElement) => {
      const rect = button.getBoundingClientRect();
      const x = event.clientX - rect.left;
      const y = event.clientY - rect.top;
      button.style.setProperty('--mouse-x', `${x}px`);
      button.style.setProperty('--mouse-y', `${y}px`);
    });
  }

  userForm = new FormGroup<UserRegistrationForm>({
    Email: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    Password: new FormControl<string | null>(null, Validators.required),
    Name: new FormControl<string | null>(null, Validators.required),
    Surname: new FormControl<string | null>(null, Validators.required),
    BirthDate: new FormControl<string | null>(null, Validators.required),
    Role: new FormControl<'User' | null>('User')
  });

  manufacturerForm = new FormGroup<ManufacturerRegistrationForm>({
    Email: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    Password: new FormControl<string | null>(null, Validators.required),
    Organization: new FormControl<string | null>(null, Validators.required),
    Role: new FormControl<'Manufacturer' | null>('Manufacturer')
  });

  navigateToLogin() {
    this.router.navigate(['/login']);
  }

  switchForm() {
    if (this.switching) return;
    
    this.switching = true;
    setTimeout(() => {
      this.isProducerForm = !this.isProducerForm;
      setTimeout(() => {
        this.switching = false;
      }, 500); 
    }, 10);
  }

  onUserSubmit() {
    if (this.userForm.valid) {
      const formValue = this.userForm.value;

      if (!formValue.Email || !formValue.Password || !formValue.Name || 
          !formValue.Surname || !formValue.BirthDate) {
        console.error('Required fields are missing');
        return;
      }

      const userData = {
        Email: formValue.Email as string,
        Password: formValue.Password as string,
        Role: 'User' as const, 
        Name: formValue.Name as string,
        Surname: formValue.Surname as string,
        BirthDate: new Date(formValue.BirthDate!).toISOString()
      };

      this.authService.createUser(userData).subscribe({
        next: (response) => {
          this.router.navigate(['/profile']);
        },
        error: (err) => {
          this.errorHandler.handleError(err, "Unable to create user");
        }
      });
    }
  }

  onProducerSubmit() {
    if (this.manufacturerForm.valid) {
      const formValue = this.manufacturerForm.value;

      if (!formValue.Email || !formValue.Password || !formValue.Organization) {
        console.error('Required fields are missing');
        return;
      }

      const userData = {
        Email: formValue.Email as string,
        Password: formValue.Password as string,
        Role: 'Manufacturer' as const, 
        Organization: formValue.Organization as string
      };

      this.authService.createUser(userData).subscribe({
        next: (response) => {
          this.router.navigate(['']);
        },
        error: (err) => {
          this.errorHandler.handleError(err, "Unable to create manufacturer");
        }
      });
    }
  }

  get animationState() {
    return this.isProducerForm ? 'producer' : 'user';
  }
}
