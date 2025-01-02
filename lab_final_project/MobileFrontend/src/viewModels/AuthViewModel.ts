import { makeObservable, observable, action, computed, runInAction } from 'mobx';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { BaseViewModel } from './BaseViewModel';
import { authApi } from '../services/api';
import { User } from '../models/types';

export class AuthViewModel extends BaseViewModel {
    @observable user: User | null = null;
    @observable token: string | null = null;
    @observable isGuest: boolean = false;

    constructor() {
        super();
        makeObservable(this);
        this.initializeAuth();
    }

    @action
    private async initializeAuth() {
        try {
            const token = await AsyncStorage.getItem('token');
            const userJson = await AsyncStorage.getItem('user');
            const isGuest = await AsyncStorage.getItem('isGuest');
            
            runInAction(() => {
                this.token = token;
                this.user = userJson && userJson !== 'undefined' ? JSON.parse(userJson) : null;
                this.isGuest = isGuest === 'true';
                this.isInitialized = true;
            });
        } catch (error) {
            console.error('Error initializing auth:', error);
            runInAction(() => {
                this.token = null;
                this.user = null;
                this.isGuest = false;
                this.isInitialized = true;
            });
        }
    }

    @action
    async login(email: string, password: string) {
        try {
            const response = await authApi.login(email, password);
            console.log('Login API response:', response);
            
            if (!response || !response.token || !response.user) {
                throw new Error('Invalid response from server');
            }
            
            // First update AsyncStorage
            await Promise.all([
                AsyncStorage.setItem('token', response.token),
                AsyncStorage.setItem('user', JSON.stringify(response.user)),
                AsyncStorage.setItem('isGuest', 'false')
            ]);
            
            // Then update observable state
            runInAction(() => {
                this.token = response.token;
                this.user = response.user;
                this.isGuest = false;
            });
            
            return true;
        } catch (error) {
            console.error('Login error:', error);
            runInAction(() => {
                this.token = null;
                this.user = null;
                this.isGuest = false;
            });
            throw error;
        }
    }

    @action
    async skipLogin() {
        try {
            // First update AsyncStorage
            await AsyncStorage.setItem('isGuest', 'true');
            await AsyncStorage.removeItem('token');
            await AsyncStorage.removeItem('user');
            
            // Then update observable state
            runInAction(() => {
                this.isGuest = true;
                this.token = null;
                this.user = null;
            });
            
            return true;
        } catch (error) {
            console.error('Skip login error:', error);
            throw error;
        }
    }

    @action
    async register(email: string, password: string) {
        try {
            const result = await this.handleApiCall(async () => {
                const response = await authApi.register(email, password);
                if (!response || !response.token || !response.user) {
                    throw new Error('Invalid response from server');
                }
                
                // First update AsyncStorage
                await Promise.all([
                    AsyncStorage.setItem('token', response.token),
                    AsyncStorage.setItem('user', JSON.stringify(response.user)),
                    AsyncStorage.setItem('isGuest', 'false')
                ]);
                
                // Then update observable state
                runInAction(() => {
                    this.token = response.token;
                    this.user = response.user;
                    this.isGuest = false;
                });
                
                return response;
            });
            
            return !!result;
        } catch (error) {
            console.error('Register error:', error);
            throw error;
        }
    }

    @action
    async logout() {
        try {
            // First update AsyncStorage
            await Promise.all([
                AsyncStorage.removeItem('token'),
                AsyncStorage.removeItem('user'),
                AsyncStorage.removeItem('isGuest')
            ]);
            
            // Then update observable state
            runInAction(() => {
                this.token = null;
                this.user = null;
                this.isGuest = false;
            });
        } catch (error) {
            console.error('Logout error:', error);
            throw error;
        }
    }

    @computed
    get isAuthenticated() {
        return !!this.token && !!this.user;
    }

    @computed
    get isAdmin() {
        return this.user?.role === 'admin';
    }
} 