import React, { useEffect, useState } from 'react';
import {
    Container,
    Typography,
    Paper,
    TextField,
    Button,
    Box,
    Stack,
    Alert,
    CircularProgress
} from '@mui/material';
import { useNavigate, useParams } from 'react-router-dom';
import { Product } from '../models/types';

export const EditProduct: React.FC = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState<string | null>(null);
    const [product, setProduct] = useState<Product | null>(null);

    useEffect(() => {
        const fetchProduct = async () => {
            try {
                const response = await fetch(`http://localhost:8000/whatever/api/Products/${id}`, {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch product');
                }

                const data = await response.json();
                setProduct(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'Failed to fetch product');
            } finally {
                setLoading(false);
            }
        };

        if (id) {
            fetchProduct();
        }
    }, [id]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!product) return;

        try {
            const response = await fetch(`http://localhost:8000/whatever/api/Products/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${localStorage.getItem('token')}`
                },
                body: JSON.stringify(product)
            });

            if (!response.ok) {
                throw new Error('Failed to update product');
            }

            setSuccess('Product updated successfully!');
            setTimeout(() => {
                navigate('/');
            }, 1500);
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to update product');
        }
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setProduct(prev => {
            if (!prev) return prev;
            if (name === 'categories') {
                return {
                    ...prev,
                    [name]: value.split(',').map(cat => cat.trim()).filter(cat => cat.length > 0)
                };
            }
            if (name === 'price') {
                return {
                    ...prev,
                    [name]: Number(value)
                };
            }
            return {
                ...prev,
                [name]: value
            };
        });
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    if (!product) {
        return (
            <Container>
                <Alert severity="error">Product not found</Alert>
            </Container>
        );
    }

    return (
        <Container maxWidth="md">
            <Paper sx={{ p: 4, mt: 4 }}>
                <Typography variant="h4" component="h1" gutterBottom>
                    Edit Product
                </Typography>

                {error && (
                    <Alert severity="error" sx={{ mb: 2 }}>
                        {error}
                    </Alert>
                )}
                {success && (
                    <Alert severity="success" sx={{ mb: 2 }}>
                        {success}
                    </Alert>
                )}

                <Box component="form" onSubmit={handleSubmit}>
                    <Stack spacing={3}>
                        <TextField
                            fullWidth
                            label="Title"
                            name="title"
                            value={product.title}
                            onChange={handleChange}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Description"
                            name="description"
                            value={product.description}
                            onChange={handleChange}
                            multiline
                            rows={4}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Image URL"
                            name="imageUrl"
                            value={product.imageUrl}
                            onChange={handleChange}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Barcode"
                            name="barcode"
                            value={product.barcode}
                            onChange={handleChange}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Price"
                            name="price"
                            type="number"
                            value={product.price}
                            onChange={handleChange}
                            required
                        />
                        <TextField
                            fullWidth
                            label="Categories (comma-separated)"
                            name="categories"
                            value={product.categories.join(', ')}
                            onChange={handleChange}
                            helperText="Enter categories separated by commas"
                            required
                        />
                        <Box sx={{ display: 'flex', gap: 2, justifyContent: 'flex-end' }}>
                            <Button
                                variant="outlined"
                                onClick={() => navigate('/')}
                            >
                                Cancel
                            </Button>
                            <Button
                                type="submit"
                                variant="contained"
                                color="primary"
                            >
                                Save Changes
                            </Button>
                        </Box>
                    </Stack>
                </Box>
            </Paper>
        </Container>
    );
}; 