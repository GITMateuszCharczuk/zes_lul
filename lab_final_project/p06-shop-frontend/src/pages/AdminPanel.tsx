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
    MenuItem,
    TextField,
    Stack,
    Alert
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

interface ProductForm {
    title: string;
    description: string;
    imageUrl: string;
    barcode: string;
    price: number;
    categories: string;
}

export const AdminPanel: React.FC = () => {
    const [tab, setTab] = useState(0);
    const [users, setUsers] = useState<User[]>([]);
    const [orders, setOrders] = useState<Order[]>([]);
    const [loading, setLoading] = useState(true);
    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
    const [selectedUserId, setSelectedUserId] = useState<string | null>(null);
    const [productForm, setProductForm] = useState<ProductForm>({
        title: '',
        description: '',
        imageUrl: '',
        barcode: '',
        price: 0,
        categories: ''
    });
    const [productError, setProductError] = useState<string | null>(null);
    const [productSuccess, setProductSuccess] = useState<string | null>(null);

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

    const handleProductSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setProductError(null);
        setProductSuccess(null);

        try {
            const categoriesArray = productForm.categories
                .split(',')
                .map(cat => cat.trim())
                .filter(cat => cat.length > 0);

            const productData = {
                title: productForm.title,
                description: productForm.description,
                imageUrl: productForm.imageUrl,
                barcode: productForm.barcode,
                price: Number(productForm.price),
                releaseDate: new Date().toISOString(),
                categories: categoriesArray
            };

            const response = await fetch('http://localhost:8000/whatever/api/Products', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify(productData)
            });

            if (!response.ok) {
                throw new Error('Failed to create product');
            }

            setProductSuccess('Product created successfully!');
            setProductForm({
                title: '',
                description: '',
                imageUrl: '',
                barcode: '',
                price: 0,
                categories: ''
            });
        } catch (err) {
            setProductError(err instanceof Error ? err.message : 'Failed to create product');
        }
    };

    const handleProductFormChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setProductForm(prev => ({
            ...prev,
            [name]: value
        }));
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
                    <Tab label="Products" />
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

                <TabPanel value={tab} index={2}>
                    <Paper sx={{ p: 3 }}>
                        <Typography variant="h6" gutterBottom>
                            Add New Product
                        </Typography>
                        {productError && (
                            <Alert severity="error" sx={{ mb: 2 }}>
                                {productError}
                            </Alert>
                        )}
                        {productSuccess && (
                            <Alert severity="success" sx={{ mb: 2 }}>
                                {productSuccess}
                            </Alert>
                        )}
                        <Box component="form" onSubmit={handleProductSubmit}>
                            <Stack spacing={2}>
                                <TextField
                                    fullWidth
                                    label="Title"
                                    name="title"
                                    value={productForm.title}
                                    onChange={handleProductFormChange}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Description"
                                    name="description"
                                    value={productForm.description}
                                    onChange={handleProductFormChange}
                                    multiline
                                    rows={4}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Image URL"
                                    name="imageUrl"
                                    value={productForm.imageUrl}
                                    onChange={handleProductFormChange}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Barcode"
                                    name="barcode"
                                    value={productForm.barcode}
                                    onChange={handleProductFormChange}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Price"
                                    name="price"
                                    type="number"
                                    value={productForm.price}
                                    onChange={handleProductFormChange}
                                    required
                                />
                                <TextField
                                    fullWidth
                                    label="Categories (comma-separated)"
                                    name="categories"
                                    value={productForm.categories}
                                    onChange={handleProductFormChange}
                                    helperText="Enter categories separated by commas (e.g., Electronics, Gadgets)"
                                    required
                                />
                                <Button
                                    type="submit"
                                    variant="contained"
                                    color="primary"
                                    size="large"
                                >
                                    Add Product
                                </Button>
                            </Stack>
                        </Box>
                    </Paper>
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