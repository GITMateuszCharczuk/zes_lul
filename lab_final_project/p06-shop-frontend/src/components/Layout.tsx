import React from 'react';
import { AppBar, Box, Container, IconButton, Menu, MenuItem, Toolbar, Typography, Badge } from '@mui/material';
import { ShoppingCart, AccountCircle } from '@mui/icons-material';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useCart } from '../contexts/CartContext';

export const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { user, logout, isAdmin } = useAuth();
    const { items } = useCart();
    const navigate = useNavigate();
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);

    const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClose = () => {
        setAnchorEl(null);
    };

    const handleLogout = () => {
        logout();
        handleClose();
        navigate('/login');
    };

    return (
        <Box sx={{ display: 'flex', flexDirection: 'column', minHeight: '100vh' }}>
            <AppBar position="static">
                <Toolbar>
                    <Typography variant="h6" component={Link} to="/" sx={{ flexGrow: 1, textDecoration: 'none', color: 'inherit' }}>
                        Online Shop {user && `(User ID: ${user.userId})`}
                    </Typography>

                    {user ? (
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                            {isAdmin && (
                                <Typography
                                    component={Link}
                                    to="/admin"
                                    sx={{ marginRight: 2, textDecoration: 'none', color: 'inherit' }}
                                >
                                    Admin Panel
                                </Typography>
                            )}
                            <IconButton
                                component={Link}
                                to="/cart"
                                color="inherit"
                                sx={{ marginRight: 2 }}
                            >
                                <Badge badgeContent={items.length} color="error">
                                    <ShoppingCart />
                                </Badge>
                            </IconButton>
                            <IconButton
                                onClick={handleMenu}
                                color="inherit"
                            >
                                <AccountCircle />
                            </IconButton>
                            <Menu
                                anchorEl={anchorEl}
                                open={Boolean(anchorEl)}
                                onClose={handleClose}
                            >
                                <MenuItem component={Link} to="/profile" onClick={handleClose}>Profile</MenuItem>
                                <MenuItem component={Link} to="/orders" onClick={handleClose}>Orders</MenuItem>
                                <MenuItem onClick={handleLogout}>Logout</MenuItem>
                            </Menu>
                        </Box>
                    ) : (
                        <Box>
                            <Typography
                                component={Link}
                                to="/login"
                                sx={{ marginRight: 2, textDecoration: 'none', color: 'inherit' }}
                            >
                                Login
                            </Typography>
                            <Typography
                                component={Link}
                                to="/register"
                                sx={{ textDecoration: 'none', color: 'inherit' }}
                            >
                                Register
                            </Typography>
                        </Box>
                    )}
                </Toolbar>
            </AppBar>

            <Container 
                component="main" 
                maxWidth={false}
                sx={{ 
                    flex: 1, 
                    py: 3,
                    px: { xs: 2, sm: 4 },
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center'
                }}
            >
                <Container maxWidth="lg">
                    {children}
                </Container>
            </Container>

            <Box component="footer" sx={{ py: 3, px: 2, mt: 'auto', backgroundColor: 'primary.main', color: 'white' }}>
                <Container maxWidth="sm">
                    <Typography variant="body2" align="center">
                        © {new Date().getFullYear()} Online Shop. All rights reserved.
                    </Typography>
                </Container>
            </Box>
        </Box>
    );
}; 