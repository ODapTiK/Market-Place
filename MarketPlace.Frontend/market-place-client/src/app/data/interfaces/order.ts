export interface Order {
    id: string;
    userId: string;
    orderPoints: OrderPoint[];
    totalPrice: number;
    status: string;     
    controlAdminId: string;           
    orderDateTime: Date;
}

export interface OrderPoint {
    id: string;
    orderId: string;
    productId: string;
    productName?: string | null;    
    productDescription?: string | null;
    productCategory?: string | null;
    productType?: string | null;
    productImage?: string | null;
    numberOfUnits: number;          
}
  