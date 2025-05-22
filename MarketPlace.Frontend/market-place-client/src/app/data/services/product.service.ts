import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Product } from '../interfaces/product';
import { ProductDto } from '../interfaces/product-dto';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  httpClient = inject(HttpClient);

  baseApiUrl = environment.apiUrls.baseApiUrl;

  getManufacturerProducts(manufacturerId: string) {
    return this.httpClient.get<Product[]>(`${this.baseApiUrl}/products/manufacturers/${manufacturerId}`);
  }

  getProducts() {
    return this.httpClient.get<Product[]>(`${this.baseApiUrl}/products`)
  }

  getProductsByIdList(payload: {productIds: string[]}) {
    return this.httpClient.put<Product[]>(`${this.baseApiUrl}/products/ids`, payload)
  }

  getProduct(id: string) {
    return this.httpClient.get<Product>(`${this.baseApiUrl}/products/${id}`);
  }

  deleteProduct(productId: string) {
    return this.httpClient.delete(`${this.baseApiUrl}/products/${productId}`);
  }

  updateProduct(updatedProduct: ProductDto) {
    return this.httpClient.put(`${this.baseApiUrl}/products`, updatedProduct);
  }

  createProduct(updatedProduct: ProductDto) {
    return this.httpClient.post(`${this.baseApiUrl}/products`, updatedProduct);
  }

  addProductToUserCart(productId: string) {
    return this.httpClient.post(`${this.baseApiUrl}/products/${productId}/user`, null);
  }

  removeProductFromUserCart(productId: string) {
    return this.httpClient.delete(`${this.baseApiUrl}/products/${productId}/user`);
  }

  createReview(productId: string, payload: {raiting: number, description: string}){
    return this.httpClient.post(`${this.baseApiUrl}/products/${productId}/reviews`, payload);
  }

  deleteReview(productId: string, reviewId: string){
    return this.httpClient.delete(`${this.baseApiUrl}/products/${productId}/reviews/${reviewId}`);
  }
}
