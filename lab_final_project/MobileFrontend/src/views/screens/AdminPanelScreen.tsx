import React, { useEffect } from 'react';
import { View, FlatList, StyleSheet, Alert, RefreshControl } from 'react-native';
import { Card, Title, Paragraph, Button, FAB } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useNavigation } from '@react-navigation/native';
import { useProducts } from '../../hooks/useStore';

const AdminPanelScreen = observer(() => {
    const navigation = useNavigation<any>();
    const productViewModel = useProducts();

    useEffect(() => {
        productViewModel.loadProducts();
    }, []);

    const handleDelete = async (productId: number) => {
        Alert.alert(
            'Confirm Delete',
            'Are you sure you want to delete this product?',
            [
                { text: 'Cancel', style: 'cancel' },
                {
                    text: 'Delete',
                    style: 'destructive',
                    onPress: async () => {
                        try {
                            await productViewModel.deleteProduct(productId);
                            Alert.alert('Success', 'Product deleted successfully');
                        } catch (error) {
                            Alert.alert('Error', productViewModel.error || 'Failed to delete product');
                        }
                    },
                },
            ]
        );
    };

    const renderProduct = ({ item }) => (
        <Card style={styles.card}>
            {item.imageUrl && (
                <Card.Cover source={{ uri: item.imageUrl }} style={styles.cardImage} />
            )}
            <Card.Content>
                <Title>{item.title}</Title>
                <Paragraph numberOfLines={2}>{item.description}</Paragraph>
                <Paragraph style={styles.price}>${item.price.toFixed(2)}</Paragraph>
            </Card.Content>
            <Card.Actions>
                <Button
                    mode="outlined"
                    onPress={() => navigation.navigate('EditProduct', { productId: item.id })}
                >
                    Edit
                </Button>
                <Button
                    mode="outlined"
                    onPress={() => handleDelete(item.id)}
                    style={styles.deleteButton}
                >
                    Delete
                </Button>
            </Card.Actions>
        </Card>
    );

    return (
        <View style={styles.container}>
            <FlatList
                data={productViewModel.products}
                renderItem={renderProduct}
                keyExtractor={item => item.id.toString()}
                contentContainerStyle={styles.list}
                refreshControl={
                    <RefreshControl
                        refreshing={productViewModel.isLoading}
                        onRefresh={() => productViewModel.loadProducts()}
                    />
                }
            />
            <FAB
                style={styles.fab}
                icon="plus"
                onPress={() => navigation.navigate('EditProduct', { productId: null })}
                disabled={productViewModel.isLoading}
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
    cardImage: {
        height: 150,
    },
    price: {
        fontSize: 16,
        fontWeight: 'bold',
        marginTop: 8,
        color: '#007AFF',
    },
    deleteButton: {
        marginLeft: 8,
    },
    fab: {
        position: 'absolute',
        margin: 16,
        right: 0,
        bottom: 0,
        backgroundColor: '#007AFF',
    },
});

export default AdminPanelScreen; 