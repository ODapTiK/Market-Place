import { Routes } from '@angular/router';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { RegisterPageComponent } from './pages/register-page/register-page.component';
import { UserProfilePageComponent } from './pages/user-profile-page/user-profile-page.component';
import { ManufacturerProfilePageComponent } from './pages/manufacturer-profile-page/manufacturer-profile-page.component';
import { userAccessGuard } from './auth/user-access.guard';
import { manufacturerAccessGuard } from './auth/manufacturerAccess.guard';
import { UserLayoutComponent } from './common-ui/user-layout/user-layout.component';
import { ManufacturerLayoutComponent } from './common-ui/manufacturer-layout/manufacturer-layout.component';
import { ManufacturerNotificationsPageComponent } from './pages/manufacturer-notifications-page/manufacturer-notifications-page.component';
import { CatalogPageComponent } from './pages/catalog-page/catalog-page.component';
import { CartPageComponent } from './pages/cart-page/cart-page.component';
import { adminAccessGuard } from './auth/admin-access.guard';
import { AdminLayoutComponent } from './common-ui/admin-layout/admin-layout.component';
import { AdminProfilePageComponent } from './pages/admin-profile-page/admin-profile-page.component';

export const routes: Routes = [
    {path:'user', component: UserLayoutComponent, canActivate: [userAccessGuard], children: [
        {path:'profile', component: UserProfilePageComponent},
        {path:'catalog', component: CatalogPageComponent},
        {path:'cart', component: CartPageComponent}
    ]},
    {path:'manufacturer', component: ManufacturerLayoutComponent, canActivate: [manufacturerAccessGuard], children: [
        {path:'profile', component: ManufacturerProfilePageComponent},
        {path:'notifications', component: ManufacturerNotificationsPageComponent}
    ]},
    {path:'admin', component: AdminLayoutComponent, canActivate: [adminAccessGuard], children: [
        {path:'profile', component: AdminProfilePageComponent}
    ]},
    {path:'login', component: LoginPageComponent},
    {path:'register', component: RegisterPageComponent}
];
