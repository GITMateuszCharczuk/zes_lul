import { makeObservable, observable, action, runInAction } from 'mobx';
import { BaseViewModel } from './BaseViewModel';
import { productsApi } from '../services/api';
import { Product } from '../models/types';

export class ProductViewModel extends BaseViewModel {
    @observable products: Product[] = [];
    @observable selectedProduct: Product | null = null;

    constructor() {
        super();
        makeObservable(this);
    }

    @action
    async loadProducts() {
        const products = await this.handleApiCall(async () => {
            const data = await productsApi.getAll();
            runInAction(() => {
                this.products = data;
            });
            return data;
        });
        return products;
    }

    @action
    async loadProduct(id: number) {
        const product = await this.handleApiCall(async () => {
            const data = await productsApi.getById(id);
            runInAction(() => {
                this.selectedProduct = data;
            });
            return data;
        });
        return product;
    }

    @action
    async createProduct(product: Omit<Product, 'id'>) {
        const newProduct = await this.handleApiCall(async () => {
            const data = await productsApi.create(product);
            runInAction(() => {
                this.products.unshift(data);
            });
            return data;
        });
        return newProduct;
    }

    @action
    async updateProduct(id: number, product: Partial<Product>) {
        const updatedProduct = await this.handleApiCall(async () => {
            const data = await productsApi.update(id, product);
            runInAction(() => {
                const index = this.products.findIndex(p => p.id === id);
                if (index !== -1) {
                    this.products[index] = data;
                }
                if (this.selectedProduct?.id === id) {
                    this.selectedProduct = data;
                }
            });
            return data;
        });
        return updatedProduct;
    }

    @action
    async deleteProduct(id: number) {
        await this.handleApiCall(async () => {
            await productsApi.delete(id);
            runInAction(() => {
                this.products = this.products.filter(p => p.id !== id);
                if (this.selectedProduct?.id === id) {
                    this.selectedProduct = null;
                }
            });
        });
    }
} 