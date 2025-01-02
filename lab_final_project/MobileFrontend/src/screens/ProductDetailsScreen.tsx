import React, { useEffect } from 'react';
import { View, StyleSheet, ScrollView } from 'react-native';
import { Card, Title, Paragraph, Button } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'ProductDetails'>;

const ProductDetailsScreen: React.FC<Props> = observer(({ route }) => {
    const { productViewModel, cartViewModel } = useStore();
    const productId = route.params?.productId;

    useEffect(() => {
        if (productId) {
            productViewModel.loadProduct(productId);
        }
    }, [productId]);

    const handleAddToCart = () => {
        if (productViewModel.selectedProduct) {
            cartViewModel.addItem(productViewModel.selectedProduct);
        }
    };

    if (!productViewModel.selectedProduct) {
        return null;
    }

    return (
        <ScrollView style={styles.container}>
            <Card style={styles.card}>
                {productViewModel.selectedProduct.imageUrl && (
                    <Card.Cover
                        source={{ uri: productViewModel.selectedProduct.imageUrl }}
                        style={styles.image}
                    />
                )}
                <Card.Content>
                    <Title style={styles.title}>{productViewModel.selectedProduct.title}</Title>
                    <Paragraph style={styles.price}>
                        ${productViewModel.selectedProduct.price.toFixed(2)}
                    </Paragraph>
                    <Paragraph style={styles.description}>
                        {productViewModel.selectedProduct.description}
                    </Paragraph>
                </Card.Content>
                <Card.Actions>
                    <Button
                        mode="contained"
                        onPress={handleAddToCart}
                        style={styles.button}
                        buttonColor="#1976d2"
                        textColor="white"
                    >
                        Add to Cart
                    </Button>
                </Card.Actions>
            </Card>
        </ScrollView>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        backgroundColor: '#fff',
    },
    card: {
        margin: 16,
        elevation: 4,
        backgroundColor: '#fff',
    },
    image: {
        height: 300,
    },
    title: {
        fontSize: 24,
        marginTop: 16,
        color: '#000',
    },
    price: {
        fontSize: 20,
        fontWeight: 'bold',
        color: '#1976d2',
        marginTop: 8,
    },
    description: {
        fontSize: 16,
        marginTop: 16,
        lineHeight: 24,
        color: '#757575',
    },
    button: {
        flex: 1,
        margin: 16,
    },
});

export default ProductDetailsScreen; 