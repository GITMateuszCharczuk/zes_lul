export interface User {
    id: string;
    email: string;
    username: string;
    role?: string;
}

export interface Product {
    id: number;
    title: string;
    description: string;
    price: number;
    imageUrl?: string;
}

export interface OrderProduct {
    productId: string;
    quantity: number;
    product: Product;
}

export interface Order {
    id: string;
    userId: string;
    orderDate: string;
    status: string;
    products: OrderProduct[];
    totalAmount: number;
}

export interface CartItem {
    product: Product;
    quantity: number;
} 