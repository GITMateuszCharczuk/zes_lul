import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Alert } from 'react-native';
import { TextInput, Button, Text } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Login'>;

const LoginScreen: React.FC<Props> = observer(({ navigation }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const { authViewModel } = useStore();

    // Monitor authentication state changes
    useEffect(() => {
        console.log('Auth state check:', {
            isAuthenticated: authViewModel.isAuthenticated,
            hasToken: !!authViewModel.token,
            hasUser: !!authViewModel.user
        });
        
        if (authViewModel.isAuthenticated) {
            console.log('Successfully authenticated, navigating to main app...');
            navigation.reset({
                index: 0,
                routes: [{ name: 'Main' }],
            });
        }
    }, [authViewModel.isAuthenticated, authViewModel.token, authViewModel.user, navigation]);

    const handleLogin = async () => {
        if (!email || !password) {
            Alert.alert('Error', 'Please fill in all fields');
            return;
        }

        try {
            console.log('Attempting login...');
            const result = await authViewModel.login(email, password);
            console.log('Login result:', result);
            if (!result) {
                Alert.alert('Error', 'Login failed - invalid credentials');
            }
        } catch (error) {
            console.log('Login error:', error);
            Alert.alert('Error', authViewModel.error || 'Login failed');
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.title}>Welcome Back</Text>
            <TextInput
                label="Email"
                value={email}
                onChangeText={setEmail}
                mode="outlined"
                keyboardType="email-address"
                autoCapitalize="none"
                style={styles.input}
                disabled={authViewModel.isLoading}
                textContentType="emailAddress"
                autoComplete="email"
            />
            <TextInput
                label="Password"
                value={password}
                onChangeText={setPassword}
                mode="outlined"
                secureTextEntry
                style={styles.input}
                disabled={authViewModel.isLoading}
                textContentType="password"
                autoComplete="password"
            />
            <Button
                mode="contained"
                onPress={handleLogin}
                loading={authViewModel.isLoading}
                style={styles.button}
                contentStyle={styles.buttonContent}
                disabled={authViewModel.isLoading}
                buttonColor="#1976d2"
                textColor="white"
            >
                Login
            </Button>
            <Button
                mode="text"
                onPress={() => navigation.navigate('Register')}
                style={styles.linkButton}
                disabled={authViewModel.isLoading}
                textColor="#1976d2"
            >
                Don't have an account? Register
            </Button>
        </View>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 20,
        justifyContent: 'center',
        backgroundColor: '#fff',
    },
    title: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 20,
        textAlign: 'center',
        color: '#1976d2',
    },
    input: {
        marginBottom: 12,
        backgroundColor: '#fff',
    },
    button: {
        marginTop: 16,
        borderRadius: 4,
    },
    buttonContent: {
        paddingVertical: 8,
    },
    linkButton: {
        marginTop: 8,
    },
    skipButton: {
        marginTop: 16,
        borderRadius: 4,
    },
});

export default LoginScreen; 