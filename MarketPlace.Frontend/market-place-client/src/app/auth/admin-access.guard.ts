import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { inject } from '@angular/core';

export const adminAccessGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if(authService.isAuth && authService.userRole?.toLocaleLowerCase() === "admin") {
    return true;
  }

  return inject(Router).createUrlTree(['login']);
};
