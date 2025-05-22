import { Component, inject } from '@angular/core';
import { UserService } from '../../data/services/user.service';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { firstValueFrom } from 'rxjs';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';

@Component({
  selector: 'app-admin-sidebar',
  imports: [SvgIconComponent, RouterModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss'
})
export class AdminSidebarComponent {
  userService = inject(UserService);
  router = inject(Router);
  authService = inject(AuthService)
  
  adminProfile = this.userService.adminProfile;

  logout() {
    this.authService.logout();
  }

  ngOnInit() {
    firstValueFrom(this.userService.getAdminProfile());
  }
}
