import React, { useEffect } from 'react';
import { View, FlatList, StyleSheet, RefreshControl } from 'react-native';
import { Card, Title, Paragraph, Text, List } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { Order, OrderProduct } from '../models/types';

type Props = NativeStackScreenProps<any, 'Orders'>;

const OrdersScreen: React.FC<Props> = observer(() => {
    const { orderViewModel } = useStore();

    useEffect(() => {
        loadOrders();
    }, []);

    const loadOrders = async () => {
        try {
            await orderViewModel.loadOrders();
            console.log('Orders loaded:', orderViewModel.orders);
        } catch (error) {
            console.error('Error loading orders:', error);
        }
    };

    const renderOrderItem = ({ item }: { item: Order }) => (
        <Card style={styles.card}>
            <Card.Content>
                <Title>Order #{item.id}</Title>
                <Paragraph>Date: {new Date(item.orderDate).toLocaleString()}</Paragraph>
                <Paragraph style={styles.status}>Status: {item.status}</Paragraph>
                <List.Section>
                    {item.products?.map((orderProduct: OrderProduct) => (
                        <List.Item
                            key={orderProduct.productId}
                            title={orderProduct.product?.title || 'Unknown Product'}
                            description={`Quantity: ${orderProduct.quantity}`}
                            right={() => (
                                <Text style={styles.price}>
                                    ${((orderProduct.product?.price || 0) * orderProduct.quantity).toFixed(2)}
                                </Text>
                            )}
                        />
                    ))}
                </List.Section>
                <View style={styles.totalContainer}>
                    <Text style={styles.totalLabel}>Total Amount:</Text>
                    <Text style={styles.totalAmount}>${(item.totalAmount || 0).toFixed(2)}</Text>
                </View>
            </Card.Content>
        </Card>
    );

    if (orderViewModel.isLoading) {
        return (
            <View style={[styles.container, styles.centerContent]}>
                <Text>Loading orders...</Text>
            </View>
        );
    }

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
                        onRefresh={loadOrders}
                    />
                }
                ListEmptyComponent={
                    <View style={styles.centerContent}>
                        <Text style={styles.emptyText}>
                            {orderViewModel.error || 'No orders found'}
                        </Text>
                    </View>
                }
            />
        </View>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    centerContent: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    list: {
        padding: 16,
        flexGrow: 1,
    },
    card: {
        marginBottom: 16,
        elevation: 4,
        backgroundColor: '#fff',
    },
    status: {
        marginTop: 4,
        fontWeight: 'bold',
        textTransform: 'capitalize',
        color: '#1976d2',
    },
    price: {
        fontSize: 16,
        fontWeight: 'bold',
        color: '#1976d2',
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
        color: '#000',
    },
    totalAmount: {
        fontSize: 20,
        fontWeight: 'bold',
        color: '#1976d2',
    },
    emptyText: {
        textAlign: 'center',
        fontSize: 18,
        marginTop: 32,
        color: '#757575',
    },
});

export default OrdersScreen; 