import React, { createContext, useContext, useState, useEffect } from 'react';
import { AuthResponse, LoginModel, RegisterModel } from '../models/types';
import { authService } from '../services/api';

interface AuthContextType {
    user: AuthResponse | null;
    login: (credentials: LoginModel) => Promise<void>;
    register: (data: RegisterModel) => Promise<void>;
    logout: () => void;
    isAdmin: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [user, setUser] = useState<AuthResponse | null>(null);
    const [isAdmin, setIsAdmin] = useState(false);

    useEffect(() => {
        const currentUser = authService.getCurrentUser();
        if (currentUser) {
            setUser(currentUser);
            setIsAdmin(currentUser.role === 'Admin');
        }
    }, []);

    const login = async (credentials: LoginModel) => {
        const response = await authService.login(credentials);
        setUser(response);
        setIsAdmin(response.role === 'Admin');
    };

    const register = async (data: RegisterModel) => {
        const response = await authService.register(data);
        setUser(response);
        setIsAdmin(response.role === 'Admin');
    };

    const logout = () => {
        authService.logout();
        setUser(null);
        setIsAdmin(false);
    };

    return (
        <AuthContext.Provider value={{ user, login, register, logout, isAdmin }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (context === undefined) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
}; 