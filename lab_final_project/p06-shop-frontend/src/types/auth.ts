export interface UserInfo {
    id: string;
    username: string;
    email: string;
    firstName?: string;
    lastName?: string;
    role: string;
}

export interface AuthState {
    user: UserInfo | null;
    token: string | null;
    refresh_token: string | null;
    roles: string[];
    isAuthenticated: boolean;
    isLoading: boolean;
}

export interface AuthCredentials {
    user?: UserInfo;
    token: string;
    refresh_token: string;
    roles: string[];
} 