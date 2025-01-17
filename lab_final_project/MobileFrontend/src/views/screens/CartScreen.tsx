import React from 'react';
import { View, FlatList, StyleSheet, Alert } from 'react-native';
import { Card, Title, Paragraph, Button, IconButton, Text } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useCart, useAuth } from '../../hooks/useStore';

const CartScreen = observer(() => {
    const cartViewModel = useCart();
    const authViewModel = useAuth();

    const handleCheckout = async () => {
        if (!authViewModel.isAuthenticated) {
            Alert.alert('Error', 'Please login to checkout');
            return;
        }

        if (cartViewModel.items.length === 0) {
            Alert.alert('Error', 'Your cart is empty');
            return;
        }

        try {
            await cartViewModel.checkout();
            Alert.alert('Success', 'Order placed successfully!');
        } catch (error) {
            Alert.alert('Error', cartViewModel.error || 'Failed to place order');
        }
    };

    const renderCartItem = ({ item }) => (
        <Card style={styles.card}>
            {item.product.imageUrl && (
                <Card.Cover source={{ uri: item.product.imageUrl }} style={styles.cardImage} />
            )}
            <Card.Content>
                <Title>{item.product.title}</Title>
                <Paragraph>${item.product.price.toFixed(2)}</Paragraph>
                <View style={styles.quantityContainer}>
                    <IconButton
                        icon="minus"
                        onPress={() => cartViewModel.updateQuantity(item.product.id, item.quantity - 1)}
                    />
                    <Text style={styles.quantity}>{item.quantity}</Text>
                    <IconButton
                        icon="plus"
                        onPress={() => cartViewModel.updateQuantity(item.product.id, item.quantity + 1)}
                    />
                </View>
                <Paragraph style={styles.itemTotal}>
                    Total: ${(item.product.price * item.quantity).toFixed(2)}
                </Paragraph>
            </Card.Content>
            <Card.Actions>
                <Button
                    mode="outlined"
                    onPress={() => cartViewModel.removeFromCart(item.product.id)}
                >
                    Remove
                </Button>
            </Card.Actions>
        </Card>
    );

    return (
        <View style={styles.container}>
            <FlatList
                data={cartViewModel.items}
                renderItem={renderCartItem}
                keyExtractor={item => item.product.id.toString()}
                contentContainerStyle={styles.list}
                ListEmptyComponent={
                    <Text style={styles.emptyText}>Your cart is empty</Text>
                }
            />
            {cartViewModel.items.length > 0 && (
                <View style={styles.footer}>
                    <Text style={styles.total}>Total: ${cartViewModel.total.toFixed(2)}</Text>
                    <Button
                        mode="contained"
                        onPress={handleCheckout}
                        loading={cartViewModel.isLoading}
                        style={styles.checkoutButton}
                    >
                        Checkout
                    </Button>
                </View>
            )}
        </View>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#f5f5f5',
    },
    list: {
        padding: 16,
    },
    card: {
        marginBottom: 16,
        elevation: 4,
    },
    cardImage: {
        height: 150,
    },
    quantityContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 8,
    },
    quantity: {
        fontSize: 18,
        marginHorizontal: 8,
    },
    itemTotal: {
        fontSize: 16,
        fontWeight: 'bold',
        marginTop: 8,
    },
    footer: {
        padding: 16,
        backgroundColor: 'white',
        elevation: 8,
    },
    total: {
        fontSize: 20,
        fontWeight: 'bold',
        marginBottom: 8,
    },
    checkoutButton: {
        padding: 4,
    },
    emptyText: {
        textAlign: 'center',
        fontSize: 18,
        marginTop: 32,
        color: 'gray',
    },
});

export default CartScreen; 