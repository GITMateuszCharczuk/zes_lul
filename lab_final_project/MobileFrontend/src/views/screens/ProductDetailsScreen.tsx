import React, { useEffect } from 'react';
import { View, ScrollView, StyleSheet, ActivityIndicator } from 'react-native';
import { Card, Title, Paragraph, Button, Text } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useRoute, useNavigation } from '@react-navigation/native';
import { useProducts, useCart, useAuth } from '../../hooks/useStore';

const ProductDetailsScreen = observer(() => {
    const route = useRoute<any>();
    const navigation = useNavigation<any>();
    const productViewModel = useProducts();
    const cartViewModel = useCart();
    const authViewModel = useAuth();

    useEffect(() => {
        productViewModel.loadProduct(route.params.productId);
    }, [route.params.productId]);

    const handleAddToCart = () => {
        if (productViewModel.selectedProduct) {
            cartViewModel.addToCart(productViewModel.selectedProduct);
            navigation.navigate('Cart');
        }
    };

    const handleEdit = () => {
        if (productViewModel.selectedProduct) {
            navigation.navigate('EditProduct', { productId: productViewModel.selectedProduct.id });
        }
    };

    if (productViewModel.isLoading) {
        return (
            <View style={styles.loadingContainer}>
                <ActivityIndicator size="large" color="#007AFF" />
            </View>
        );
    }

    if (!productViewModel.selectedProduct) {
        return (
            <View style={styles.errorContainer}>
                <Text>Product not found</Text>
                {productViewModel.error && (
                    <Text style={styles.errorText}>{productViewModel.error}</Text>
                )}
            </View>
        );
    }

    const product = productViewModel.selectedProduct;

    return (
        <ScrollView style={styles.container}>
            <Card style={styles.card}>
                {product.imageUrl && (
                    <Card.Cover source={{ uri: product.imageUrl }} style={styles.image} />
                )}
                <Card.Content>
                    <Title style={styles.title}>{product.title}</Title>
                    <Paragraph style={styles.price}>${product.price.toFixed(2)}</Paragraph>
                    <Paragraph style={styles.description}>{product.description}</Paragraph>
                </Card.Content>
                <Card.Actions style={styles.actions}>
                    {authViewModel.isAdmin ? (
                        <Button mode="contained" onPress={handleEdit}>
                            Edit Product
                        </Button>
                    ) : (
                        <Button mode="contained" onPress={handleAddToCart}>
                            Add to Cart
                        </Button>
                    )}
                </Card.Actions>
            </Card>
        </ScrollView>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#f5f5f5',
    },
    loadingContainer: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    errorContainer: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    errorText: {
        color: 'red',
        marginTop: 8,
    },
    card: {
        margin: 16,
        elevation: 4,
    },
    image: {
        height: 300,
    },
    title: {
        fontSize: 24,
        marginTop: 16,
    },
    price: {
        fontSize: 20,
        fontWeight: 'bold',
        color: '#007AFF',
        marginVertical: 8,
    },
    description: {
        fontSize: 16,
        lineHeight: 24,
        marginTop: 8,
    },
    actions: {
        justifyContent: 'flex-end',
        padding: 16,
    },
});

export default ProductDetailsScreen; 