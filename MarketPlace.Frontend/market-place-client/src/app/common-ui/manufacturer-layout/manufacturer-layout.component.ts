import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ManufacturerSidebarComponent } from '../manufacturer-sidebar/manufacturer-sidebar.component';

@Component({
  selector: 'app-manufacturer-layout',
  imports: [RouterOutlet, ManufacturerSidebarComponent],
  templateUrl: './manufacturer-layout.component.html',
  styleUrl: './manufacturer-layout.component.scss'
})
export class ManufacturerLayoutComponent {

}
