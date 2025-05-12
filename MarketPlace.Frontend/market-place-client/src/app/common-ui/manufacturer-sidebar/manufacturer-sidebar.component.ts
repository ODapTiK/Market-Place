import { Component, inject } from '@angular/core';
import { SvgIconComponent } from '../svg-icon/svg-icon.component';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../data/services/user.service';
import { AuthService } from '../../auth/auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-manufacturer-sidebar',
  imports: [SvgIconComponent, RouterModule],
  templateUrl: './manufacturer-sidebar.component.html',
  styleUrl: './manufacturer-sidebar.component.scss'
})
export class ManufacturerSidebarComponent {
  userService = inject(UserService);
  router = inject(Router);
  authService = inject(AuthService)
  
  manufacturerProfile = this.userService.manufacturerProfile;

  logout() {
    this.authService.logout();
  }

  ngOnInit() {
    firstValueFrom(this.userService.getManufacturerProfile());
  }
}
