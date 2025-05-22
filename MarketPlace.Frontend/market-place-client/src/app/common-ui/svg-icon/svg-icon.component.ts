  import { Component, Input } from '@angular/core';

  @Component({
    selector: 'svg[icon]',
    standalone: true,
    template: '<svg:use [attr.href]="href" fill="currentColor"/>',
    host: {
      'xmlns': 'http://www.w3.org/2000/svg',
      'fill': 'currentColor',
      'width': '24',
      'height': '24',
      '[attr.viewBox]': 'viewBox'
    }
  })
  export class SvgIconComponent {
    @Input() icon!: string;
    @Input() size: number = 24;
    
    private customViewBoxes: {[key: string]: string} = {
      'notification': '0 0 24 24'
    };

    get viewBox(): string {
      return this.customViewBoxes[this.icon] || '0 0 24 24';
    }

    get href(): string {
      return `assets/icons/icons.svg#${this.icon}`;
    }
  }

