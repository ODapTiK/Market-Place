import { Component, inject } from '@angular/core';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { UserService } from '../../data/services/user.service';
import { firstValueFrom } from 'rxjs';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../auth/auth.service';

@Component({
  selector: 'app-sidebar',
  imports: [SvgIconComponent, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  userService = inject(UserService);
  router = inject(Router);
  authService = inject(AuthService)
  
  userProfile = this.userService.userProfile;

  logout() {
    this.authService.deleteTokens();
    this.authService.logout();
  }

  ngOnInit() {
    firstValueFrom(this.userService.getUserProfile());
  }
}
