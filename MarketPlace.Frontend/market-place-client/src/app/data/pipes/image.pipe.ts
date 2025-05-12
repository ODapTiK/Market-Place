import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'image'
})
export class ImagePipe implements PipeTransform {

    transform(logo: string | null | undefined, defaultImage: string = '/assets/default-images/user-logo.jpg'): string {
      if (!logo) {
        return defaultImage;
      }
      
      if (logo.startsWith('data:image')) {
        return logo;
      }
      
      const imageType = this.detectImageType(logo);
  
      return `data:image/${imageType};base64,${logo}`;
    }
  
    private detectImageType(base64: string): string {
      if (base64.startsWith('/9j') || base64.startsWith('/9j/4')) {
        return 'jpeg';
      }
      else if (base64.startsWith('iVBORw0KGgo')) {
        return 'png';
      }
      else if (base64.startsWith('R0lGODdh') || base64.startsWith('R0lGODlh')) {
        return 'gif';
      }
      else if (base64.startsWith('UklGR')) {
        return 'webp';
      }
  
      return 'jpeg';
    }
}
