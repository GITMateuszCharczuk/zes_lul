import React, { useEffect, useState } from 'react';
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
    Chip,
    CircularProgress,
    Box
} from '@mui/material';
import { Order } from '../models/types';
import { orderService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';

export const Orders: React.FC = () => {
    const [orders, setOrders] = useState<Order[]>([]);
    const [loading, setLoading] = useState(true);
    const { user } = useAuth();

    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const data = await orderService.getAll();
                setOrders(data);
            } catch (error) {
                console.error('Failed to fetch orders:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchOrders();
    }, []);

    const getStatusColor = (status: string) => {
        switch (status.toLowerCase()) {
            case 'pending':
                return 'warning';
            case 'processing':
                return 'info';
            case 'shipped':
                return 'success';
            case 'delivered':
                return 'success';
            case 'cancelled':
                return 'error';
            default:
                return 'default';
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Container>
            <Typography variant="h4" component="h1" gutterBottom>
                My Orders
            </Typography>
            {orders.length === 0 ? (
                <Paper sx={{ p: 3, textAlign: 'center' }}>
                    <Typography>You haven't placed any orders yet.</Typography>
                </Paper>
            ) : (
                <TableContainer component={Paper}>
                    <Table>
                        <TableHead>
                            <TableRow>
                                <TableCell>Order ID</TableCell>
                                <TableCell>Date</TableCell>
                                <TableCell>Items</TableCell>
                                <TableCell align="right">Total Amount</TableCell>
                                <TableCell align="center">Status</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {orders.map((order) => (
                                <TableRow key={order.id}>
                                    <TableCell>{order.id}</TableCell>
                                    <TableCell>
                                        {new Date(order.orderDate || '').toLocaleDateString()}
                                    </TableCell>
                                    <TableCell>
                                        {order.items.map((item, index) => (
                                            <Typography key={index} variant="body2">
                                                {item.quantity}x - ${item.priceAtOrder.toFixed(2)}
                                            </Typography>
                                        ))}
                                    </TableCell>
                                    <TableCell align="right">
                                        ${order.totalAmount.toFixed(2)}
                                    </TableCell>
                                    <TableCell align="center">
                                        <Chip
                                            label={order.status}
                                            color={getStatusColor(order.status) as any}
                                            size="small"
                                        />
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
        </Container>
    );
}; 