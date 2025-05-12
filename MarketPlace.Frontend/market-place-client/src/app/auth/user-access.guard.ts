import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const userAccessGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  console.log(authService.isAuth);
  console.log(authService.userRole);
  console.log(authService.cookieService.getAll());
  if(authService.isAuth && authService.userRole?.toLocaleLowerCase() === "user") {
    return true;
  }

  return inject(Router).createUrlTree(['login']);
};

