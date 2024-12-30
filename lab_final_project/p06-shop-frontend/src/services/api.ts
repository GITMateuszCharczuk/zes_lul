import axios from 'axios';
import { AuthResponse, LoginModel, Order, Product, RegisterModel, User } from '../models/types';

interface ServiceResponse<T> {
    data: T;
    success: boolean;
    message?: string;
}

const API_URL = 'http://localhost:8000/whatever/api';

const api = axios.create({
    baseURL: API_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    withCredentials: true
});

// Add token to requests if available
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Add response interceptor to handle errors and unwrap data
api.interceptors.response.use(
    response => {
        // If the response has a data.data structure (service response), unwrap it
        if (response.data && 'data' in response.data && 'success' in response.data) {
            if (!response.data.success) {
                return Promise.reject(new Error(response.data.message || 'Operation failed'));
            }
            return response.data.data;
        }
        return response.data;
    },
    error => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export const authService = {
    login: async (credentials: LoginModel): Promise<AuthResponse> => {
        try {
            const response = await api.post<ServiceResponse<AuthResponse>>('/Auth/login', credentials);
            const data = response as unknown as AuthResponse;
            localStorage.setItem('token', data.token);
            localStorage.setItem('user', JSON.stringify(data));
            return data;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    },

    register: async (data: RegisterModel): Promise<AuthResponse> => {
        try {
            const response = await api.post<ServiceResponse<AuthResponse>>('/Auth/register', data);
            const authData = response as unknown as AuthResponse;
            localStorage.setItem('token', authData.token);
            localStorage.setItem('user', JSON.stringify(authData));
            return authData;
        } catch (error) {
            console.error('Registration error:', error);
            throw error;
        }
    },

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
    },

    getCurrentUser: (): AuthResponse | null => {
        const user = localStorage.getItem('user');
        return user ? JSON.parse(user) : null;
    },
};

export const productService = {
    getAll: async (): Promise<Product[]> => {
        const response = await api.get<ServiceResponse<Product[]>>('/Products');
        return response as unknown as Product[];
    },

    getById: async (id: string): Promise<Product> => {
        const response = await api.get<ServiceResponse<Product>>(`/Products/${id}`);
        return response as unknown as Product;
    },
};

export const orderService = {
    getAll: async (): Promise<Order[]> => {
        const response = await api.get<ServiceResponse<Order[]>>('/Orders');
        return response as unknown as Order[];
    },

    getById: async (id: string): Promise<Order> => {
        const response = await api.get<ServiceResponse<Order>>(`/Orders/${id}`);
        return response as unknown as Order;
    },

    create: async (order: Order): Promise<Order> => {
        const response = await api.post<ServiceResponse<Order>>('/Orders', order);
        return response as unknown as Order;
    },

    updateStatus: async (id: string, status: string): Promise<void> => {
        await api.put(`/Orders/${id}/status`, JSON.stringify(status));
    },

    delete: async (id: string): Promise<void> => {
        await api.delete(`/Orders/${id}`);
    },
};

export const userService = {
    getAll: async (): Promise<User[]> => {
        const response = await api.get<ServiceResponse<User[]>>('/Users');
        return response as unknown as User[];
    },

    getById: async (id: string): Promise<User> => {
        const response = await api.get<ServiceResponse<User>>(`/Users/${id}`);
        return response as unknown as User;
    },

    promoteToAdmin: async (userId: string): Promise<void> => {
        await api.post(`/Auth/promote/${userId}`);
    },

    demoteToCustomer: async (userId: string): Promise<void> => {
        await api.post(`/Auth/demote/${userId}`);
    },
}; 