import { makeObservable, observable, action, computed, runInAction } from 'mobx';
import { BaseViewModel } from './BaseViewModel';
import { CartItem, Product } from '../models/types';
import AsyncStorage from '@react-native-async-storage/async-storage';

export class CartViewModel extends BaseViewModel {
    @observable items: CartItem[] = [];

    constructor() {
        super();
        makeObservable(this);
        this.loadCart();
    }

    @action
    private async loadCart() {
        try {
            const cartJson = await AsyncStorage.getItem('cart');
            if (cartJson) {
                runInAction(() => {
                    this.items = JSON.parse(cartJson);
                });
            }
        } catch (error) {
            console.error('Error loading cart:', error);
        } finally {
            runInAction(() => {
                this.isInitialized = true;
            });
        }
    }

    private async saveCart() {
        try {
            await AsyncStorage.setItem('cart', JSON.stringify(this.items));
        } catch (error) {
            console.error('Error saving cart:', error);
        }
    }

    @action
    addItem(product: Product, quantity: number = 1) {
        runInAction(() => {
            const existingItem = this.items.find(item => item.product.id === product.id);
            if (existingItem) {
                existingItem.quantity += quantity;
            } else {
                this.items.push({ product, quantity });
            }
        });
        this.saveCart();
    }

    @action
    removeItem(productId: number) {
        runInAction(() => {
            this.items = this.items.filter(item => item.product.id !== productId);
        });
        this.saveCart();
    }

    @action
    updateQuantity(productId: number, quantity: number) {
        if (quantity <= 0) {
            this.removeItem(productId);
            return;
        }

        runInAction(() => {
            const item = this.items.find(item => item.product.id === productId);
            if (item) {
                item.quantity = quantity;
            }
        });
        this.saveCart();
    }

    @action
    clearCart() {
        runInAction(() => {
            this.items = [];
        });
        this.saveCart();
    }

    @computed
    get totalItems() {
        return this.items.reduce((total, item) => total + item.quantity, 0);
    }

    @computed
    get totalAmount() {
        return this.items.reduce((total, item) => total + (item.product.price * item.quantity), 0);
    }
} 