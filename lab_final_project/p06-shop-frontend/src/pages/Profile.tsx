import React, { useEffect, useState } from 'react';
import {
    Container,
    Typography,
    Paper,
    Grid,
    Box,
    CircularProgress,
    Alert
} from '@mui/material';
import { useAuth } from '../contexts/AuthContext';

interface UserDetails {
    id: string;
    username: string;
    email: string;
    firstName: string;
    lastName: string;
    role: string;
}

export const Profile: React.FC = () => {
    const { user } = useAuth();
    const [userDetails, setUserDetails] = useState<UserDetails | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchUserDetails = async () => {
            try {
                const response = await fetch(`http://localhost:8000/whatever/api/Users/${user?.userId}`, {
                    headers: {
                        'Authorization': `Bearer ${localStorage.getItem('token')}`
                    }
                });
                
                if (!response.ok) {
                    throw new Error('Failed to fetch user details');
                }
                
                const data = await response.json();
                setUserDetails(data);
            } catch (err) {
                setError(err instanceof Error ? err.message : 'An error occurred');
            } finally {
                setLoading(false);
            }
        };

        if (user?.userId) {
            fetchUserDetails();
        }
    }, [user?.userId]);

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" minHeight="200px">
                <CircularProgress />
            </Box>
        );
    }

    if (error) {
        return (
            <Container maxWidth="sm">
                <Alert severity="error" sx={{ mt: 2 }}>
                    {error}
                </Alert>
            </Container>
        );
    }

    return (
        <Container maxWidth="sm">
            <Paper elevation={3} sx={{ p: 4, mt: 4 }}>
                <Typography variant="h4" component="h1" gutterBottom>
                    Profile
                </Typography>
                
                {userDetails && (
                    <Grid container spacing={2}>
                        <Grid item xs={12}>
                            <Typography variant="subtitle1" color="text.secondary">
                                Username
                            </Typography>
                            <Typography variant="body1">
                                {userDetails.username}
                            </Typography>
                        </Grid>
                        
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" color="text.secondary">
                                First Name
                            </Typography>
                            <Typography variant="body1">
                                {userDetails.firstName}
                            </Typography>
                        </Grid>
                        
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" color="text.secondary">
                                Last Name
                            </Typography>
                            <Typography variant="body1">
                                {userDetails.lastName}
                            </Typography>
                        </Grid>
                        
                        <Grid item xs={12}>
                            <Typography variant="subtitle1" color="text.secondary">
                                Email
                            </Typography>
                            <Typography variant="body1">
                                {userDetails.email}
                            </Typography>
                        </Grid>
                        
                        <Grid item xs={12}>
                            <Typography variant="subtitle1" color="text.secondary">
                                Role
                            </Typography>
                            <Typography variant="body1">
                                {userDetails.role}
                            </Typography>
                        </Grid>
                        
                        <Grid item xs={12}>
                            <Typography variant="subtitle1" color="text.secondary">
                                User ID
                            </Typography>
                            <Typography variant="body1">
                                {userDetails.id}
                            </Typography>
                        </Grid>
                    </Grid>
                )}
            </Paper>
        </Container>
    );
}; 