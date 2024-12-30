import React, { useEffect, useState } from 'react';
import {
    Container,
    Typography,
    Paper,
    Tabs,
    Tab,
    Box,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Button,
    Chip,
    CircularProgress,
    IconButton,
    Menu,
    MenuItem
} from '@mui/material';
import { MoreVert as MoreVertIcon } from '@mui/icons-material';
import { Order, User } from '../models/types';
import { orderService, userService } from '../services/api';

interface TabPanelProps {
    children?: React.ReactNode;
    index: number;
    value: number;
}

const TabPanel = (props: TabPanelProps) => {
    const { children, value, index, ...other } = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`simple-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && <Box sx={{ p: 3 }}>{children}</Box>}
        </div>
    );
};

export const AdminPanel: React.FC = () => {
    const [tab, setTab] = useState(0);
    const [users, setUsers] = useState<User[]>([]);
    const [orders, setOrders] = useState<Order[]>([]);
    const [loading, setLoading] = useState(true);
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const [usersData, ordersData] = await Promise.all([
                    userService.getAll(),
                    orderService.getAll()
                ]);
                setUsers(usersData);
                setOrders(ordersData);
            } catch (error) {
                console.error('Failed to fetch data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, []);

    const handleMenuClick = (event: React.MouseEvent<HTMLElement>, userId: string) => {
        setAnchorEl(event.currentTarget);
        setSelectedUserId(userId);
    };

    const handleMenuClose = () => {
        setAnchorEl(null);
        setSelectedUserId(null);
    };

    const handlePromoteToAdmin = async () => {
        if (selectedUserId) {
            try {
                await userService.promoteToAdmin(selectedUserId);
                setUsers(users.map(user =>
                    user.id === selectedUserId
                        ? { ...user, role: 'Admin' }
                        : user
                ));
            } catch (error) {
                console.error('Failed to promote user:', error);
            }
        }
        handleMenuClose();
    };

    const handleDemoteToCustomer = async () => {
        if (selectedUserId) {
            try {
                await userService.demoteToCustomer(selectedUserId);
                setUsers(users.map(user =>
                    user.id === selectedUserId
                        ? { ...user, role: 'Customer' }
                        : user
                ));
            } catch (error) {
                console.error('Failed to demote user:', error);
            }
        }
        handleMenuClose();
    };

    const handleUpdateOrderStatus = async (orderId: string, newStatus: string) => {
        try {
            await orderService.updateStatus(orderId, newStatus);
            setOrders(orders.map(order =>
                order.id === orderId
                    ? { ...order, status: newStatus }
                    : order
            ));
        } catch (error) {
            console.error('Failed to update order status:', error);
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
                Admin Panel
            </Typography>
            <Paper>
                <Tabs value={tab} onChange={(_, newValue) => setTab(newValue)}>
                    <Tab label="Users" />
                    <Tab label="Orders" />
                </Tabs>

                <TabPanel value={tab} index={0}>
                    <TableContainer>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Username</TableCell>
                                    <TableCell>Email</TableCell>
                                    <TableCell>Name</TableCell>
                                    <TableCell>Role</TableCell>
                                    <TableCell>Actions</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {users.map((user) => (
                                    <TableRow key={user.id}>
                                        <TableCell>{user.username}</TableCell>
                                        <TableCell>{user.email}</TableCell>
                                        <TableCell>{`${user.firstName} ${user.lastName}`}</TableCell>
                                        <TableCell>
                                            <Chip
                                                label={user.role}
                                                color={user.role === 'Admin' ? 'primary' : 'default'}
                                                size="small"
                                            />
                                        </TableCell>
                                        <TableCell>
                                            <IconButton
                                                onClick={(e) => handleMenuClick(e, user.id!)}
                                            >
                                                <MoreVertIcon />
                                            </IconButton>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                </TabPanel>

                <TabPanel value={tab} index={1}>
                    <TableContainer>
                        <Table>
                            <TableHead>
                                <TableRow>
                                    <TableCell>Order ID</TableCell>
                                    <TableCell>User</TableCell>
                                    <TableCell>Date</TableCell>
                                    <TableCell align="right">Total Amount</TableCell>
                                    <TableCell>Status</TableCell>
                                    <TableCell>Actions</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {orders.map((order) => (
                                    <TableRow key={order.id}>
                                        <TableCell>{order.id}</TableCell>
                                        <TableCell>
                                            {users.find(u => u.id === order.userId)?.username || 'Unknown'}
                                        </TableCell>
                                        <TableCell>
                                            {new Date(order.orderDate || '').toLocaleDateString()}
                                        </TableCell>
                                        <TableCell align="right">
                                            ${order.totalAmount.toFixed(2)}
                                        </TableCell>
                                        <TableCell>
                                            <Chip
                                                label={order.status}
                                                color={
                                                    order.status === 'Pending'
                                                        ? 'warning'
                                                        : order.status === 'Shipped'
                                                            ? 'success'
                                                            : 'default'
                                                }
                                                size="small"
                                            />
                                        </TableCell>
                                        <TableCell>
                                            <Button
                                                size="small"
                                                onClick={() => handleUpdateOrderStatus(order.id!, 'Shipped')}
                                                disabled={order.status === 'Shipped'}
                                            >
                                                Mark as Shipped
                                            </Button>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    </TableContainer>
                </TabPanel>
            </Paper>

            <Menu
                anchorEl={anchorEl}
                open={Boolean(anchorEl)}
                onClose={handleMenuClose}
            >
                <MenuItem onClick={handlePromoteToAdmin}>Promote to Admin</MenuItem>
                <MenuItem onClick={handleDemoteToCustomer}>Demote to Customer</MenuItem>
            </Menu>
        </Container>
    );
}; 