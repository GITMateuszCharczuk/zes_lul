import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ThemeProvider, CssBaseline } from '@mui/material';
import { AuthProvider } from './contexts/AuthContext';
import { CartProvider } from './contexts/CartContext';
import { Layout } from './components/Layout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { theme } from './theme';

// Pages
import { Home } from './pages/Home';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Cart } from './pages/Cart';
import { Profile } from './pages/Profile';
import { Orders } from './pages/Orders';
import { AdminPanel } from './pages/AdminPanel';
import { ProductDetails } from './pages/ProductDetails';

function App() {
    return (
        <ThemeProvider theme={theme}>
            <CssBaseline />
            <AuthProvider>
                <CartProvider>
                    <Router>
                        <Layout>
                            <Routes>
                                <Route path="/" element={<Home />} />
                                <Route path="/login" element={<Login />} />
                                <Route path="/register" element={<Register />} />
                                <Route path="/product/:id" element={<ProductDetails />} />
                                <Route
                                    path="/cart"
                                    element={
                                        <ProtectedRoute>
                                            <Cart />
                                        </ProtectedRoute>
                                    }
                                />
                                <Route
                                    path="/profile"
                                    element={
                                        <ProtectedRoute>
                                            <Profile />
                                        </ProtectedRoute>
                                    }
                                />
                                <Route
                                    path="/orders"
                                    element={
                                        <ProtectedRoute>
                                            <Orders />
                                        </ProtectedRoute>
                                    }
                                />
                                <Route
                                    path="/admin"
                                    element={
                                        <ProtectedRoute requireAdmin>
                                            <AdminPanel />
                                        </ProtectedRoute>
                                    }
                                />
                            </Routes>
                        </Layout>
                    </Router>
                </CartProvider>
            </AuthProvider>
        </ThemeProvider>
    );
}

export default App; 