import React from 'react';
import { View, FlatList, StyleSheet, Alert } from 'react-native';
import { Card, Title, Paragraph, Button, Text, IconButton } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Cart'>;

const CartScreen: React.FC<Props> = observer(({ navigation }) => {
    const { cartViewModel, orderViewModel } = useStore();

    const handleCheckout = async () => {
        if (cartViewModel.items.length === 0) {
            Alert.alert('Error', 'Your cart is empty');
            return;
        }

        try {
            const orderItems = cartViewModel.items.map(item => ({
                productId: item.product.id,
                quantity: item.quantity,
            }));

            await orderViewModel.createOrder(orderItems);
            cartViewModel.clearCart();
            Alert.alert('Success', 'Order placed successfully');
            navigation.navigate('Orders');
        } catch (error) {
            Alert.alert('Error', 'Failed to place order');
        }
    };

    const renderCartItem = ({ item }) => (
        <Card style={styles.card}>
            {item.product.imageUrl && (
                <Card.Cover source={{ uri: item.product.imageUrl }} style={styles.cardImage} />
            )}
            <Card.Content>
                <Title>{item.product.title}</Title>
                <Paragraph style={styles.price}>
                    ${(item.product.price * item.quantity).toFixed(2)}
                </Paragraph>
                <View style={styles.quantityContainer}>
                    <IconButton
                        icon="minus"
                        size={20}
                        onPress={() => cartViewModel.updateQuantity(item.product.id, item.quantity - 1)}
                        iconColor="#1976d2"
                    />
                    <Text style={styles.quantity}>{item.quantity}</Text>
                    <IconButton
                        icon="plus"
                        size={20}
                        onPress={() => cartViewModel.updateQuantity(item.product.id, item.quantity + 1)}
                        iconColor="#1976d2"
                    />
                </View>
            </Card.Content>
            <Card.Actions>
                <Button
                    mode="outlined"
                    onPress={() => cartViewModel.removeItem(item.product.id)}
                    textColor="#1976d2"
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
                <Card style={styles.totalCard}>
                    <Card.Content>
                        <Title style={styles.totalTitle}>Total: ${cartViewModel.totalAmount.toFixed(2)}</Title>
                    </Card.Content>
                    <Card.Actions>
                        <Button
                            mode="contained"
                            onPress={handleCheckout}
                            style={styles.checkoutButton}
                            buttonColor="#1976d2"
                            textColor="white"
                        >
                            Checkout
                        </Button>
                    </Card.Actions>
                </Card>
            )}
        </View>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    list: {
        padding: 16,
    },
    card: {
        marginBottom: 16,
        elevation: 4,
        backgroundColor: '#fff',
    },
    cardImage: {
        height: 150,
    },
    price: {
        fontSize: 18,
        fontWeight: 'bold',
        color: '#1976d2',
    },
    quantityContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        marginTop: 8,
    },
    quantity: {
        fontSize: 18,
        marginHorizontal: 8,
        color: '#000',
    },
    totalCard: {
        margin: 16,
        elevation: 4,
        backgroundColor: '#fff',
    },
    totalTitle: {
        color: '#1976d2',
    },
    checkoutButton: {
        flex: 1,
        marginHorizontal: 8,
    },
    emptyText: {
        textAlign: 'center',
        fontSize: 18,
        marginTop: 32,
        color: '#757575',
    },
});

export default CartScreen; 