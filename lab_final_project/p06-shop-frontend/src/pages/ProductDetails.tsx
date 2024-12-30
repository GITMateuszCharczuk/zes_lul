import React, { useEffect, useState } from 'react';
import {
    Container,
    Grid,
    Typography,
    Paper,
    Button,
    Box,
    CircularProgress,
    TextField,
    Alert
} from '@mui/material';
import { useParams, useNavigate } from 'react-router-dom';
import { Product } from '../models/types';
import { productService } from '../services/api';
import { useCart } from '../contexts/CartContext';

export const ProductDetails: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const [product, setProduct] = useState<Product | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [quantity, setQuantity] = useState(1);
    const navigate = useNavigate();
    const { addToCart } = useCart();

    useEffect(() => {
        const fetchProduct = async () => {
            try {
                if (!id) {
                    throw new Error('Product ID is required');
                }
                const data = await productService.getById(id);
                setProduct(data);
            } catch (error) {
                console.error('Failed to fetch product:', error);
                setError('Failed to load product details');
            } finally {
                setLoading(false);
            }
        };

        fetchProduct();
    }, [id]);

    const handleQuantityChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const value = parseInt(event.target.value);
        if (!isNaN(value) && value >= 1) {
            setQuantity(value);
        }
    };

    const handleAddToCart = () => {
        if (product) {
            addToCart(product, quantity);
            navigate('/cart');
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    if (error || !product) {
        return (
            <Container>
                <Alert severity="error" sx={{ mt: 2 }}>
                    {error || 'Product not found'}
                </Alert>
                <Button
                    variant="contained"
                    onClick={() => navigate('/')}
                    sx={{ mt: 2 }}
                >
                    Back to Products
                </Button>
            </Container>
        );
    }

    return (
        <Container>
            <Grid container spacing={4}>
                <Grid item xs={12} md={6}>
                    <Paper
                        sx={{
                            height: 400,
                            backgroundImage: `url(https://picsum.photos/seed/${product.id}/800/600)`,
                            backgroundSize: 'cover',
                            backgroundPosition: 'center',
                        }}
                    />
                </Grid>
                <Grid item xs={12} md={6}>
                    <Typography variant="h4" component="h1" gutterBottom>
                        {product.title}
                    </Typography>
                    <Typography variant="body1" paragraph>
                        {product.description}
                    </Typography>
                    <Typography variant="h5" color="primary" gutterBottom>
                        ${product.price.toFixed(2)}
                    </Typography>
                    <Box sx={{ my: 3 }}>
                        <Typography variant="subtitle1" gutterBottom>
                            Quantity:
                        </Typography>
                        <TextField
                            type="number"
                            value={quantity}
                            onChange={handleQuantityChange}
                            inputProps={{ min: 1 }}
                            size="small"
                            sx={{ width: 100 }}
                        />
                    </Box>
                    <Button
                        variant="contained"
                        color="primary"
                        size="large"
                        onClick={handleAddToCart}
                        fullWidth
                    >
                        Add to Cart
                    </Button>
                    <Button
                        variant="outlined"
                        onClick={() => navigate('/')}
                        fullWidth
                        sx={{ mt: 2 }}
                    >
                        Back to Products
                    </Button>
                </Grid>
            </Grid>
        </Container>
    );
}; 