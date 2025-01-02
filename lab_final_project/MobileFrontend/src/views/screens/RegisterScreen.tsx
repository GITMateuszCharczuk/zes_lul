import React, { useState } from 'react';
import { View, StyleSheet, Alert } from 'react-native';
import { TextInput, Button, Text } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useAuth } from '../../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Register'>;

const RegisterScreen: React.FC<Props> = observer(({ navigation }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const authViewModel = useAuth();

    const handleRegister = async () => {
        if (!email || !password || !confirmPassword) {
            Alert.alert('Error', 'Please fill in all fields');
            return;
        }

        if (password !== confirmPassword) {
            Alert.alert('Error', 'Passwords do not match');
            return;
        }

        try {
            await authViewModel.register(email, password);
        } catch (error) {
            Alert.alert('Error', authViewModel.error || 'Registration failed');
        }
    };

    return (
        <View style={styles.container}>
            <Text style={styles.title}>Create Account</Text>
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
            <TextInput
                label="Confirm Password"
                value={confirmPassword}
                onChangeText={setConfirmPassword}
                mode="outlined"
                secureTextEntry
                style={styles.input}
                disabled={authViewModel.isLoading}
            />
            <Button
                mode="contained"
                onPress={handleRegister}
                loading={authViewModel.isLoading}
                style={styles.button}
            >
                Register
            </Button>
            <Button
                mode="text"
                onPress={() => navigation.navigate('Login')}
                style={styles.linkButton}
                disabled={authViewModel.isLoading}
            >
                Already have an account? Login
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

export default RegisterScreen; 