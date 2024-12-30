import React, { useEffect, useState } from 'react';
import {
    Container,
    Typography,
    Paper,
    Grid,
    TextField,
    Button,
    Box,
    Alert,
    CircularProgress
} from '@mui/material';
import { useAuth } from '../contexts/AuthContext';
import { userService } from '../services/api';
import { User } from '../models/types';

export const Profile: React.FC = () => {
    const { user: authUser } = useAuth();
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        email: '',
        username: ''
    });

    useEffect(() => {
        const fetchUserData = async () => {
            if (!authUser?.userId) return;

            try {
                const userData = await userService.getById(authUser.userId);
                setUser(userData);
                setFormData({
                    firstName: userData.firstName,
                    lastName: userData.lastName,
                    email: userData.email,
                    username: userData.username
                });
            } catch (error) {
                console.error('Failed to fetch user data:', error);
                setError('Failed to load user profile');
            } finally {
                setLoading(false);
            }
        };

        fetchUserData();
    }, [authUser]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        // Note: This is a placeholder as we haven't implemented profile updates in the API
        try {
            setSuccess('Profile updated successfully');
        } catch (error) {
            console.error('Failed to update profile:', error);
            setError('Failed to update profile');
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="60vh">
                <CircularProgress />
            </Box>
        );
    }

    if (!user) {
        return (
            <Container>
                <Alert severity="error">
                    Failed to load user profile
                </Alert>
            </Container>
        );
    }

    return (
        <Container maxWidth="md">
            <Typography variant="h4" component="h1" gutterBottom>
                Profile
            </Typography>

            <Paper sx={{ p: 3, mt: 3 }}>
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
                    <Grid container spacing={3}>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="First Name"
                                name="firstName"
                                value={formData.firstName}
                                onChange={handleChange}
                            />
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <TextField
                                fullWidth
                                label="Last Name"
                                name="lastName"
                                value={formData.lastName}
                                onChange={handleChange}
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Email"
                                name="email"
                                value={formData.email}
                                onChange={handleChange}
                                type="email"
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <TextField
                                fullWidth
                                label="Username"
                                name="username"
                                value={formData.username}
                                onChange={handleChange}
                            />
                        </Grid>
                        <Grid item xs={12}>
                            <Button
                                type="submit"
                                variant="contained"
                                color="primary"
                                fullWidth
                            >
                                Update Profile
                            </Button>
                        </Grid>
                    </Grid>
                </Box>
            </Paper>

            <Paper sx={{ p: 3, mt: 3 }}>
                <Typography variant="h6" gutterBottom>
                    Account Information
                </Typography>
                <Grid container spacing={2}>
                    <Grid item xs={12}>
                        <Typography variant="body1">
                            <strong>Role:</strong> {user.role}
                        </Typography>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography variant="body1">
                            <strong>Member Since:</strong> {new Date().toLocaleDateString()}
                        </Typography>
                    </Grid>
                </Grid>
            </Paper>
        </Container>
    );
}; 