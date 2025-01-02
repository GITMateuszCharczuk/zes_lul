import React, { useState } from 'react';
import { View, StyleSheet, Alert } from 'react-native';
import { TextInput, Button, Text } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useAuth } from '../../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Login'>;

const LoginScreen: React.FC<Props> = observer(({ navigation }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const authViewModel = useAuth();

    const handleLogin = async () => {
        if (!email || !password) {
            Alert.alert('Error', 'Please fill in all fields');
            return;
        }

        try {
            await authViewModel.login(email, password);
        } catch (error) {
            // Error is already handled in ViewModel
            Alert.alert('Error', authViewModel.error || 'Invalid credentials');
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
            />
            <TextInput
                label="Password"
                value={password}
                onChangeText={setPassword}
                mode="outlined"
                secureTextEntry
                style={styles.input}
                disabled={authViewModel.isLoading}
            />
            <Button
                mode="contained"
                onPress={handleLogin}
                loading={authViewModel.isLoading}
                style={styles.button}
            >
                Login
            </Button>
            <Button
                mode="text"
                onPress={() => navigation.navigate('Register')}
                style={styles.linkButton}
                disabled={authViewModel.isLoading}
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
    },
    title: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 20,
        textAlign: 'center',
    },
    input: {
        marginBottom: 12,
    },
    button: {
        marginTop: 16,
        padding: 4,
    },
    linkButton: {
        marginTop: 8,
    },
});

export default LoginScreen; 