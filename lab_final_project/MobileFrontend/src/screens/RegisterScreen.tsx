import React, { useState } from 'react';
import { View, StyleSheet, Alert, ScrollView } from 'react-native';
import { TextInput, Button, Text } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Register'>;

const RegisterScreen: React.FC<Props> = observer(({ navigation }) => {
    const [email, setEmail] = useState('');
    const [username, setUsername] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const { authViewModel } = useStore();

    const handleRegister = async () => {
        if (!email || !password || !confirmPassword || !username || !firstName || !lastName) {
            Alert.alert('Error', 'Please fill in all fields');
            return;
        }

        if (password !== confirmPassword) {
            Alert.alert('Error', 'Passwords do not match');
            return;
        }

        try {
            await authViewModel.register({
                email,
                username,
                firstName,
                lastName,
                password
            });
        } catch (error) {
            Alert.alert('Error', authViewModel.error || 'Registration failed');
        }
    };

    return (
        <ScrollView style={styles.container}>
            <View style={styles.content}>
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
                    label="Username"
                    value={username}
                    onChangeText={setUsername}
                    mode="outlined"
                    autoCapitalize="none"
                    style={styles.input}
                    disabled={authViewModel.isLoading}
                />
                <TextInput
                    label="First Name"
                    value={firstName}
                    onChangeText={setFirstName}
                    mode="outlined"
                    style={styles.input}
                    disabled={authViewModel.isLoading}
                />
                <TextInput
                    label="Last Name"
                    value={lastName}
                    onChangeText={setLastName}
                    mode="outlined"
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
                    buttonColor="#1976d2"
                    textColor="white"
                >
                    Register
                </Button>
                <Button
                    mode="text"
                    onPress={() => navigation.navigate('Login')}
                    style={styles.linkButton}
                    disabled={authViewModel.isLoading}
                    textColor="#1976d2"
                >
                    Already have an account? Login
                </Button>
            </View>
        </ScrollView>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    content: {
        padding: 20,
    },
    title: {
        fontSize: 24,
        fontWeight: 'bold',
        marginBottom: 20,
        textAlign: 'center',
        color: '#000',
    },
    input: {
        marginBottom: 12,
        backgroundColor: '#fff',
    },
    button: {
        marginTop: 16,
        borderRadius: 4,
    },
    linkButton: {
        marginTop: 8,
    },
});

export default RegisterScreen; 