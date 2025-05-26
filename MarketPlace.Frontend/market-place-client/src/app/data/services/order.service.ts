import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { Order } from '../interfaces/order';
import { Cart } from '../interfaces/cart';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  httpClient = inject(HttpClient);

  baseApiOrderServiceUrl = environment.apiUrls.baseApiUrl;

  userOrders = signal<Order[] | null>(null);
  userCart = signal<Cart | null>(null);

  getUserOrders() {
    return this.httpClient.get<Order[]>(`${this.baseApiOrderServiceUrl}/orders`)
      .pipe(
        tap(response => this.userOrders.set(response))
      )
  }

  getOrdersByIdList(payload: {orderIds: string[]}){
    return this.httpClient.patch<Order[]>(`${this.baseApiOrderServiceUrl}/orders`, payload);
  }

  setOrderStatusReady(id: string){
    return this.httpClient.patch(`${this.baseApiOrderServiceUrl}/orders/${id}`, null);
  }

  createOrder(payload: {points: any, controlAdminId: string}) {
    return this.httpClient.post(`${this.baseApiOrderServiceUrl}/orders`, payload)
  }

  loadUserCart() {
    return this.httpClient.get<Cart>(`${this.baseApiOrderServiceUrl}/carts`)
      .pipe(
        tap(response => this.userCart.set(response))
      );
  }

  cancelOrder(orderId: string) {
    return this.httpClient.delete(`${this.baseApiOrderServiceUrl}/orders/${orderId}`);
  }
}

