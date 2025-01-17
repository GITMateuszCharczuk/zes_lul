import { makeAutoObservable, runInAction } from 'mobx';
import { BaseViewModel } from './BaseViewModel';
import { User } from '../models/types';
import { authApi } from '../services/api';
import AsyncStorage from '@react-native-async-storage/async-storage';

export class AuthViewModel extends BaseViewModel {
    user: User | null = null;
    isInitialized: boolean = false;

    constructor() {
        super();
        makeAutoObservable(this);
        this.checkAuth();
    }

    private async checkAuth() {
        try {
            const token = await AsyncStorage.getItem('token');
            const userData = await AsyncStorage.getItem('user');
            if (token && userData) {
                runInAction(() => {
                    this.user = JSON.parse(userData);
                });
            }
        } catch (error) {
            console.error('Error checking auth:', error);
        } finally {
            runInAction(() => {
                this.isInitialized = true;
            });
        }
    }

    async login(email: string, password: string) {
        await this.handleApiCall(async () => {
            const { token, user } = await authApi.login(email, password);
            await AsyncStorage.setItem('token', token);
            await AsyncStorage.setItem('user', JSON.stringify(user));
            runInAction(() => {
                this.user = user;
            });
        });
    }

    async register(email: string, password: string) {
        await this.handleApiCall(async () => {
            const { token, user } = await authApi.register(email, password);
            await AsyncStorage.setItem('token', token);
            await AsyncStorage.setItem('user', JSON.stringify(user));
            runInAction(() => {
                this.user = user;
            });
        });
    }

    async logout() {
        await this.handleApiCall(async () => {
            await AsyncStorage.removeItem('token');
            await AsyncStorage.removeItem('user');
            runInAction(() => {
                this.user = null;
            });
        });
    }

    get isAuthenticated() {
        return !!this.user;
    }

    get isAdmin() {
        return this.user?.role === 'admin';
    }
} 