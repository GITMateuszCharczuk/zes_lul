import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { Product, Order, User } from '../models/types';
import { Platform } from 'react-native';

const getBaseUrl = () => {
    if (Platform.OS === 'web') {
        return 'http://localhost:8000/whatever/api';
    }
    // For Android emulator
    if (Platform.OS === 'android') {
        return 'http://10.0.2.2:8000/whatever/api';
    }
    // For iOS simulator
    if (Platform.OS === 'ios') {
        return 'http://localhost:8000/whatever/api';
    }
    return 'http://localhost:8000/whatever/api';
};

const API_URL = getBaseUrl();

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Add token to requests
api.interceptors.request.use(async (config) => {
    const token = await AsyncStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Add response interceptor to handle errors
api.interceptors.response.use(
    response => response.data,
    error => {
        if (error.response?.status === 401) {
            AsyncStorage.removeItem('token');
            AsyncStorage.removeItem('user');
            // Handle navigation to login in your app
        }
        return Promise.reject(error.response?.data?.message || error.message);
    }
);

export const authApi = {
    login: async (email: string, password: string): Promise<{ token: string; user: User }> => {
        const response = await api.post('/Auth/login', { email, password }) as {
            success: boolean;
            message: string;
            token: string;
            userId: string;
            username: string;
            role: string;
        };
        console.log('Raw API response:', response);
        if (!response || !response.success || !response.token) {
            throw new Error(response?.message || 'Invalid response from server');
        }
        
        const user = {
            id: response.userId,
            email: email,
            username: response.username,
            role: response.role
        };
        
        // Store both user and userId in AsyncStorage
        await Promise.all([
            AsyncStorage.setItem('userId', response.userId),
            AsyncStorage.setItem('user', JSON.stringify(user))
        ]);
        
        return {
            token: response.token,
            user
        };
    },
    register: async (data: { 
        email: string; 
        username: string;
        firstName: string;
        lastName: string;
        password: string;
    }) => {
        const response = await api.post('/Auth/register', data) as {
            success: boolean;
            message: string;
            token: string;
            userId: string;
            username: string;
            role: string;
        };
        
        if (!response || !response.success || !response.token || !response.userId) {
            throw new Error(response?.message || 'Invalid response from server');
        }
        
        const user = {
            id: response.userId,
            email: data.email,
            username: data.username,
            role: response.role
        };

        // Store user data in AsyncStorage
        await Promise.all([
            AsyncStorage.setItem('token', response.token),
            AsyncStorage.setItem('user', JSON.stringify(user))
        ]);

        return {
            token: response.token,
            user
        };
    },
};

export const productsApi = {
    getAll: async () => {
        const response = await api.get<Product[]>('/Products');
        return response;
    },
    getById: async (id: number) => {
        const response = await api.get<Product>(`/Products/${id}`);
        return response;
    },
    create: async (product: Omit<Product, 'id'>) => {
        const response = await api.post<Product>('/Products', product);
        return response;
    },
    update: async (id: number, product: Partial<Product>) => {
        const response = await api.put<Product>(`/Products/${id}`, product);
        return response;
    },
    delete: async (id: number) => {
        await api.delete(`/Products/${id}`);
    },
};

export const ordersApi = {
    getAll: async () => {
        const response = await api.get<Order[]>('/Orders');
        return response;
    },
    create: async (products: { productId: string; quantity: number }[]) => {
        const userJson = await AsyncStorage.getItem('user');
        if (!userJson) {
            throw new Error('User not found');
        }
        const user = JSON.parse(userJson);
        const response = await api.post<Order>('/Orders', {
            items: products,
            userId: user.id
        });
        return response;
    },
    getUserOrders: async () => {
        const userJson = await AsyncStorage.getItem('user');
        if (!userJson) {
            throw new Error('User not found');
        }
        const user = JSON.parse(userJson);
        const response = await api.get<Order[]>(`/Orders/user/${user.id}`);
        return response;
    },
}; 