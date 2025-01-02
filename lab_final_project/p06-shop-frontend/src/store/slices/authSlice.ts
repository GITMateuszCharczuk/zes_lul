import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { AuthState, AuthCredentials, UserInfo } from '../../types/auth';

const initialState: AuthState = {
    user: null,
    token: null,
    refresh_token: null,
    roles: [],
    isAuthenticated: false,
    isLoading: false,
};

const authSlice = createSlice({
    name: 'auth',
    initialState,
    reducers: {
        setCredentials: (state, action: PayloadAction<AuthCredentials>) => {
            if (action.payload.user) state.user = action.payload.user;
            state.token = action.payload.token;
            state.refresh_token = action.payload.refresh_token;
            state.roles = action.payload.roles;
            state.isAuthenticated = true;
            state.isLoading = false;
        },
        setToken: (state, action: PayloadAction<string>) => {
            state.token = action.payload;
        },
        setUser: (state, action: PayloadAction<UserInfo>) => {
            state.user = action.payload;
        },
        setLoading: (state, action: PayloadAction<boolean>) => {
            state.isLoading = action.payload;
        },
        logout: (state) => {
            state.user = null;
            state.token = null;
            state.refresh_token = null;
            state.roles = [];
            state.isAuthenticated = false;
            state.isLoading = false;
        },
    },
});

// Action creators
export const { setCredentials, setToken, setUser, logout, setLoading } = authSlice.actions;

// Selectors
export const selectCurrentUser = (state: { auth: AuthState }) => state.auth.user;
export const selectIsAuthenticated = (state: { auth: AuthState }) => state.auth.isAuthenticated;
export const selectIsAdmin = (state: { auth: AuthState }) => 
    state.auth.roles.includes('Admin');
export const selectAuthToken = (state: { auth: AuthState }) => state.auth.token;
export const selectRefreshToken = (state: { auth: AuthState }) => state.auth.refresh_token;
export const selectIsLoading = (state: { auth: AuthState }) => state.auth.isLoading;
export const selectUserRoles = (state: { auth: AuthState }) => state.auth.roles;

export default authSlice.reducer; 