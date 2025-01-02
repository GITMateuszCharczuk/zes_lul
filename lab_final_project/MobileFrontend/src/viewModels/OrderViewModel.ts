import { makeObservable, observable, action, runInAction } from 'mobx';
import { BaseViewModel } from './BaseViewModel';
import { ordersApi } from '../services/api';
import { Order } from '../models/types';

export class OrderViewModel extends BaseViewModel {
    @observable orders: Order[] = [];

    constructor() {
        super();
        makeObservable(this);
    }

    @action
    async loadOrders() {
        try {
            const data = await ordersApi.getUserOrders();
            console.log('Orders data:', data);
            runInAction(() => {
                this.orders = Array.isArray(data) ? data : [];
                this.error = null;
            });
            return data;
        } catch (error) {
            console.error('Error loading orders:', error);
            runInAction(() => {
                this.orders = [];
                this.error = error instanceof Error ? error.message : 'Failed to load orders';
            });
            throw error;
        }
    }

    @action
    async createOrder(products: { productId: string; quantity: number }[]) {
        try {
            const data = await ordersApi.create(products);
            console.log('Created order:', data);
            runInAction(() => {
                if (data) {
                    this.orders.unshift(data);
                }
                this.error = null;
            });
            return data;
        } catch (error) {
            console.error('Error creating order:', error);
            runInAction(() => {
                this.error = error instanceof Error ? error.message : 'Failed to create order';
            });
            throw error;
        }
    }
} 