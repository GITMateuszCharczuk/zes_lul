import React, { useEffect, useState } from 'react';
import {
    Grid,
    Card,
    CardContent,
    CardMedia,
    Typography,
    Button,
    CardActions,
    Container,
    CircularProgress,
    Box,
    Alert
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { Product } from '../models/types';
import { productService } from '../services/api';
import { useCart } from '../contexts/CartContext';

export const Home: React.FC = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();
    const { addToCart } = useCart();

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const data = await productService.getAll();
                setProducts(data || []);
            } catch (error) {
                console.error('Failed to fetch products:', error);
                setError('Failed to load products. Please try again later.');
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    const handleProductClick = (productId: string) => {
        navigate(`/product/${productId}`);
    };

    const handleAddToCart = (product: Product, event: React.MouseEvent) => {
        event.stopPropagation();
        addToCart(product);
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Container>
                <Alert severity="error" sx={{ mt: 2 }}>
                    {error}
                </Alert>
            </Container>
        );
    }

    return (
        <Container>
            <Typography variant="h4" component="h1" gutterBottom>
                Products
            </Typography>
            <Grid container spacing={3}>
                {products && products.length > 0 ? (
                    products.map((product) => (
                        <Grid item xs={12} sm={6} md={4} key={product.id}>
                            <Card
                                onClick={() => handleProductClick(product.id)}
                                sx={{
                                    height: '100%',
                                    display: 'flex',
                                    flexDirection: 'column',
                                    cursor: 'pointer',
                                    '&:hover': {
                                        transform: 'translateY(-4px)',
                                        boxShadow: '0 4px 12px rgba(0,0,0,0.15)',
                                        transition: 'all 0.3s ease',
                                    },
                                }}
                            >
                                <CardMedia
                                    component="img"
                                    height="200"
                                    image={`https://picsum.photos/seed/${product.id}/400/300`}
                                    alt={product.title}
                                />
                                <CardContent sx={{ flexGrow: 1 }}>
                                    <Typography gutterBottom variant="h6" component="h2">
                                        {product.title}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary" noWrap>
                                        {product.description}
                                    </Typography>
                                    <Typography variant="h6" color="primary" sx={{ mt: 2 }}>
                                        ${product.price.toFixed(2)}
                                    </Typography>
                                </CardContent>
                                <CardActions>
                                    <Button
                                        size="small"
                                        color="primary"
                                        onClick={(e) => handleAddToCart(product, e)}
                                    >
                                        Add to Cart
                                    </Button>
                                </CardActions>
                            </Card>
                        </Grid>
                    ))
                ) : (
                    <Grid item xs={12}>
                        <Typography variant="h6" align="center" color="text.secondary">
                            No products available.
                        </Typography>
                    </Grid>
                )}
            </Grid>
        </Container>
    );
}; 