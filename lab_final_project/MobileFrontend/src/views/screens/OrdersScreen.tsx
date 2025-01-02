import React, { useEffect } from 'react';
import { View, FlatList, StyleSheet, RefreshControl } from 'react-native';
import { Card, Title, Paragraph, Text, List } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useOrders } from '../../hooks/useStore';
import { Order } from '../../models/types';

const OrdersScreen = observer(() => {
    const orderViewModel = useOrders();

    useEffect(() => {
        orderViewModel.loadOrders();
    }, []);

    const renderOrderItem = ({ item }: { item: Order }) => (
        <Card style={styles.card}>
            <Card.Content>
                <Title>Order #{item.id}</Title>
                <Paragraph>Date: {new Date(item.createdAt).toLocaleDateString()}</Paragraph>
                <Paragraph style={styles.status}>Status: {item.status}</Paragraph>
                <List.Section>
                    <List.Subheader>Products</List.Subheader>
                    {item.products.map((orderProduct) => (
                        <List.Item
                            key={orderProduct.productId}
                            title={orderProduct.product.title}
                            description={`Quantity: ${orderProduct.quantity}`}
                            right={() => (
                                <Text style={styles.price}>
                                    ${(orderProduct.product.price * orderProduct.quantity).toFixed(2)}
                                </Text>
                            )}
                        />
                    ))}
                </List.Section>
                <View style={styles.totalContainer}>
                    <Text style={styles.totalLabel}>Total Amount:</Text>
                    <Text style={styles.totalAmount}>${item.totalAmount.toFixed(2)}</Text>
                </View>
            </Card.Content>
        </Card>
    );

    return (
        <View style={styles.container}>
            <FlatList
                data={orderViewModel.orders}
                renderItem={renderOrderItem}
                keyExtractor={item => item.id.toString()}
                contentContainerStyle={styles.list}
                refreshControl={
                    <RefreshControl
                        refreshing={orderViewModel.isLoading}
                        onRefresh={() => orderViewModel.loadOrders()}
                    />
                }
                ListEmptyComponent={
                    <Text style={styles.emptyText}>
                        {orderViewModel.error || 'No orders found'}
                    </Text>
                }
            />
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
    status: {
        marginTop: 4,
        fontWeight: 'bold',
        textTransform: 'capitalize',
    },
    price: {
        fontSize: 16,
        fontWeight: 'bold',
    },
    totalContainer: {
        flexDirection: 'row',
        justifyContent: 'space-between',
        alignItems: 'center',
        marginTop: 16,
        paddingTop: 16,
        borderTopWidth: 1,
        borderTopColor: '#e0e0e0',
    },
    totalLabel: {
        fontSize: 18,
        fontWeight: 'bold',
    },
    totalAmount: {
        fontSize: 20,
        fontWeight: 'bold',
        color: '#007AFF',
    },
    emptyText: {
        textAlign: 'center',
        fontSize: 18,
        marginTop: 32,
        color: 'gray',
    },
});

export default OrdersScreen; 