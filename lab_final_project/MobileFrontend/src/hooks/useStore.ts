import React from 'react';
import { RootStore } from '../store/RootStore';

const StoreContext = React.createContext<RootStore | null>(null);

export const StoreProvider: React.FC<{ children: React.ReactNode; value: RootStore }> = ({ children, value }) => {
    return React.createElement(StoreContext.Provider, { value }, children);
};

export const useStore = () => {
    const store = React.useContext(StoreContext);
    if (!store) {
        throw new Error('useStore must be used within a StoreProvider');
    }
    return store;
};

// Convenience hooks for specific ViewModels
export const useAuth = () => {
    return useStore().authViewModel;
};

export const useCart = () => {
    return useStore().cartViewModel;
};

export const useProducts = () => {
    return useStore().productViewModel;
};

export const useOrders = () => {
    return useStore().orderViewModel;
}; 