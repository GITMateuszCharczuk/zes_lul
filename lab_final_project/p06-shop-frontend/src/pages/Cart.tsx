import React from 'react';
import {
    Container,
    Typography,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    IconButton,
    Button,
    Box,
    TextField
} from '@mui/material';
import { Add as AddIcon, Remove as RemoveIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { useCart } from '../contexts/CartContext';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';
import { orderService } from '../services/api';

export const Cart: React.FC = () => {
    const { items, updateQuantity, removeFromCart, total, clearCart } = useCart();
    const { user } = useAuth();
    const navigate = useNavigate();

    const handleQuantityChange = (productId: string, newQuantity: number) => {
        if (newQuantity >= 1) {
            updateQuantity(productId, newQuantity);
        }
    };

    const handleCheckout = async () => {
        if (!user) {
            navigate('/login');
            return;
        }

        try {
            const order = {
                userId: user.userId,
                items: items.map(item => ({
                    productId: item.product.id,
                    quantity: item.quantity,
                    priceAtOrder: item.product.price
                })),
                totalAmount: total,
                status: 'Pending'
            };

            await orderService.create(order);
            clearCart();
            navigate('/orders');
        } catch (error) {
            console.error('Failed to create order:', error);
        }
    };

    if (items.length === 0) {
        return (
            <Container>
                <Typography variant="h4" component="h1" gutterBottom>
                    Shopping Cart
                </Typography>
                <Paper sx={{ p: 3, textAlign: 'center' }}>
                    <Typography>Your cart is empty</Typography>
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={() => navigate('/')}
                        sx={{ mt: 2 }}
                    >
                        Continue Shopping
                    </Button>
                </Paper>
            </Container>
        );
    }

    return (
        <Container>
            <Typography variant="h4" component="h1" gutterBottom>
                Shopping Cart
            </Typography>
            <TableContainer component={Paper}>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Product</TableCell>
                            <TableCell align="right">Price</TableCell>
                            <TableCell align="center">Quantity</TableCell>
                            <TableCell align="right">Total</TableCell>
                            <TableCell align="center">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {items.map((item) => (
                            <TableRow key={item.product.id}>
                                <TableCell component="th" scope="row">
                                    <Typography variant="subtitle1">{item.product.title}</Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        {item.product.description}
                                    </Typography>
                                </TableCell>
                                <TableCell align="right">${item.product.price.toFixed(2)}</TableCell>
                                <TableCell align="center">
                                    <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                                        <IconButton
                                            size="small"
                                            onClick={() => handleQuantityChange(item.product.id, item.quantity - 1)}
                                        >
                                            <RemoveIcon />
                                        </IconButton>
                                        <TextField
                                            size="small"
                                            value={item.quantity}
                                            onChange={(e) => {
                                                const value = parseInt(e.target.value);
                                                if (!isNaN(value)) {
                                                    handleQuantityChange(item.product.id, value);
                                                }
                                            }}
                                            inputProps={{
                                                style: { textAlign: 'center' },
                                                min: 1
                                            }}
                                            sx={{ width: '60px', mx: 1 }}
                                        />
                                        <IconButton
                                            size="small"
                                            onClick={() => handleQuantityChange(item.product.id, item.quantity + 1)}
                                        >
                                            <AddIcon />
                                        </IconButton>
                                    </Box>
                                </TableCell>
                                <TableCell align="right">
                                    ${(item.product.price * item.quantity).toFixed(2)}
                                </TableCell>
                                <TableCell align="center">
                                    <IconButton
                                        color="error"
                                        onClick={() => removeFromCart(item.product.id)}
                                    >
                                        <DeleteIcon />
                                    </IconButton>
                                </TableCell>
                            </TableRow>
                        ))}
                        <TableRow>
                            <TableCell colSpan={3}>
                                <Typography variant="h6">Total</Typography>
                            </TableCell>
                            <TableCell align="right">
                                <Typography variant="h6">${total.toFixed(2)}</Typography>
                            </TableCell>
                            <TableCell />
                        </TableRow>
                    </TableBody>
                </Table>
            </TableContainer>
            <Box sx={{ mt: 3, display: 'flex', justifyContent: 'flex-end', gap: 2 }}>
                <Button
                    variant="outlined"
                    onClick={() => navigate('/')}
                >
                    Continue Shopping
                </Button>
                <Button
                    variant="contained"
                    color="primary"
                    onClick={handleCheckout}
                >
                    Proceed to Checkout
                </Button>
            </Box>
        </Container>
    );
}; 