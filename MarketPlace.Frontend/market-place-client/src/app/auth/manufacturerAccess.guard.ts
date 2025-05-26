import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const manufacturerAccessGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);

  if(authService.isAuth && authService.userRole?.toLocaleLowerCase() === "manufacturer") {
    return true;
  }

  return inject(Router).createUrlTree(['login']);
};
