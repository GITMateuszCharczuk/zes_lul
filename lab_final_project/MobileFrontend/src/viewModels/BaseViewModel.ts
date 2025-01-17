import { makeObservable, observable, action, runInAction } from 'mobx';

export class BaseViewModel {
    @observable isLoading: boolean = false;
    @observable error: string | null = null;
    @observable isInitialized: boolean = false;

    constructor() {
        makeObservable(this);
    }

    @action
    protected async handleApiCall<T>(apiCall: () => Promise<T>): Promise<T | null> {
        try {
            runInAction(() => {
                this.isLoading = true;
                this.error = null;
            });

            const result = await apiCall();

            runInAction(() => {
                this.isLoading = false;
                this.isInitialized = true;
            });

            return result;
        } catch (error) {
            runInAction(() => {
                this.isLoading = false;
                this.error = error instanceof Error ? error.message : 'An error occurred';
                this.isInitialized = true;
            });
            return null;
        }
    }
} 