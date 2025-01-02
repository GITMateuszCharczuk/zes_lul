export interface BaseViewModel {
    isLoading: boolean;
    error: string | null;
    setLoading(loading: boolean): void;
    setError(error: string | null): void;
} 