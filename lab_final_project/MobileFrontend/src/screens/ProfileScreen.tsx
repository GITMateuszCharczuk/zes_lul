import React from 'react';
import { View, StyleSheet, Alert } from 'react-native';
import { Button, Text, Card, Title, Paragraph } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Profile'>;

const ProfileScreen: React.FC<Props> = observer(() => {
    const { authViewModel } = useStore();

    const handleLogout = async () => {
        try {
            await authViewModel.logout();
        } catch (error) {
            Alert.alert('Error', 'Failed to logout');
        }
    };

    return (
        <View style={styles.container}>
            <Card style={styles.card}>
                <Card.Content>
                    <Title style={styles.title}>Profile Information</Title>
                    <Paragraph style={styles.email}>Email: {authViewModel.user?.email}</Paragraph>
                    <Paragraph style={styles.username}>Username: {authViewModel.user?.username}</Paragraph>
                    {authViewModel.user?.role && (
                        <Paragraph style={styles.role}>Role: {authViewModel.user.role}</Paragraph>
                    )}
                </Card.Content>
            </Card>
            <Button
                mode="contained"
                onPress={handleLogout}
                style={styles.logoutButton}
                buttonColor="#1976d2"
                textColor="white"
            >
                Logout
            </Button>
        </View>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: '#fff',
    },
    card: {
        marginBottom: 16,
        elevation: 4,
        backgroundColor: '#fff',
    },
    title: {
        fontSize: 20,
        fontWeight: 'bold',
        marginBottom: 12,
        color: '#1976d2',
    },
    email: {
        fontSize: 16,
        marginTop: 8,
        color: '#000',
    },
    username: {
        fontSize: 16,
        marginTop: 4,
        color: '#000',
    },
    role: {
        fontSize: 16,
        marginTop: 4,
        textTransform: 'capitalize',
        color: '#1976d2',
    },
    logoutButton: {
        marginTop: 16,
    },
});

export default ProfileScreen; 