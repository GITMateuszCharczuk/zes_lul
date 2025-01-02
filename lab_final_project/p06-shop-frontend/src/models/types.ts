export interface Product {
    id: string;
    title: string;
    description: string;
    imageUrl: string;
    barcode: string;
    price: number;
    releaseDate: string;
    categories: string[];
}

export interface OrderItem {
    productId: string;
    quantity: number;
    priceAtOrder: number;
}

export interface Order {
    id?: string;
    userId: string;
    items: OrderItem[];
    orderDate?: string;
    totalAmount: number;
    status: string;
}

export interface User {
    id?: string;
    email: string;
    username: string;
    firstName: string;
    lastName: string;
    role: string;
}

export interface LoginModel {
    email: string;
    password: string;
}

export interface RegisterModel {
    email: string;
    password: string;
    username: string;
    firstName: string;
    lastName: string;
}

export interface AuthResponse {
    token: string;
    userId: string;
    username: string;
    role: string;
} 