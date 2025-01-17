import React, { useEffect } from 'react';
import { View, FlatList, StyleSheet, RefreshControl } from 'react-native';
import { Card, Title, Paragraph, Button } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'Products'>;

const HomeScreen: React.FC<Props> = observer(({ navigation }) => {
    const { productViewModel, cartViewModel } = useStore();

    useEffect(() => {
        productViewModel.loadProducts();
    }, []);

    const handleAddToCart = (productId: number) => {
        const product = productViewModel.products.find(p => p.id === productId);
        if (product) {
            cartViewModel.addItem(product);
        }
    };

    const renderProduct = ({ item }) => (
        <Card style={styles.card} onPress={() => navigation.navigate('ProductDetails', { productId: item.id })}>
            {item.imageUrl && (
                <Card.Cover source={{ uri: item.imageUrl }} style={styles.cardImage} />
            )}
            <Card.Content>
                <Title>{item.title}</Title>
                <Paragraph numberOfLines={2} style={styles.description}>{item.description}</Paragraph>
                <Paragraph style={styles.price}>${item.price.toFixed(2)}</Paragraph>
            </Card.Content>
            <Card.Actions>
                <Button
                    mode="contained"
                    onPress={() => handleAddToCart(item.id)}
                    buttonColor="#1976d2"
                    textColor="white"
                >
                    Add to Cart
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
        height: 200,
    },
    description: {
        color: '#757575',
        marginVertical: 8,
    },
    price: {
        fontSize: 18,
        fontWeight: 'bold',
        color: '#1976d2',
        marginTop: 8,
    },
});

export default HomeScreen; 