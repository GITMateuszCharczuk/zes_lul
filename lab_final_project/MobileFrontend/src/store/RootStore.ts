import { makeObservable, observable } from 'mobx';
import { AuthViewModel } from '../viewModels/AuthViewModel';
import { CartViewModel } from '../viewModels/CartViewModel';
import { ProductViewModel } from '../viewModels/ProductViewModel';
import { OrderViewModel } from '../viewModels/OrderViewModel';

export class RootStore {
    @observable authViewModel: AuthViewModel;
    @observable cartViewModel: CartViewModel;
    @observable productViewModel: ProductViewModel;
    @observable orderViewModel: OrderViewModel;

    constructor() {
        this.authViewModel = new AuthViewModel();
        this.cartViewModel = new CartViewModel();
        this.productViewModel = new ProductViewModel();
        this.orderViewModel = new OrderViewModel();
        makeObservable(this);
    }
}

// Create a single instance of the store
export const rootStore = new RootStore(); 