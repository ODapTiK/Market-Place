import { Component, ElementRef, HostListener, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators  } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../auth/auth.service';
import { LoginForm } from '../../data/interfaces/login-form';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  authService = inject(AuthService);
  router = inject(Router);

  isPasswordVisible = signal<boolean>(false);

  constructor(private elRef: ElementRef) {}

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent) {
    const button = this.elRef.nativeElement.querySelector('.button');
    if (button) {
      const rect = button.getBoundingClientRect();
      const x = event.clientX - rect.left;
      const y = event.clientY - rect.top;
      button.style.setProperty('--mouse-x', `${x}px`);
      button.style.setProperty('--mouse-y', `${y}px`);
    }
  }

  form = new FormGroup<LoginForm>({
    email: new FormControl<string | null>(null, [Validators.required, Validators.email]),
    password: new FormControl<string | null>(null, Validators.required)
  });

  navigateToRegister() {
    this.router.navigate(['/register']);
  }

  onSubmit() {
    if(this.form.valid){
      this.authService.login({
        email: this.form.value.email!,
        password: this.form.value.password!
      }).subscribe(res => {
        if(this.authService.userRole?.toLocaleLowerCase() === "user") {
          this.router.navigate(['/user/profile']);
        }
        else if(this.authService.userRole?.toLocaleLowerCase() === "manufacturer") {
          this.router.navigate(['/manufacturer/profile']);
        }
        else if(this.authService.userRole?.toLocaleLowerCase() === "admin") {
          this.router.navigate(['/admin/profile']);
        }
        else {
          this.authService.logout();
        }
      });
    }
  }
}
