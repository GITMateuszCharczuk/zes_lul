import React, { useEffect } from 'react';
import { View, FlatList, StyleSheet, RefreshControl } from 'react-native';
import { Card, Title, Paragraph, Button, Searchbar } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useNavigation } from '@react-navigation/native';
import { useProducts, useCart } from '../../hooks/useStore';
import { Product } from '../../models/types';

const HomeScreen = observer(() => {
    const navigation = useNavigation<any>();
    const productViewModel = useProducts();
    const cartViewModel = useCart();

    useEffect(() => {
        productViewModel.loadProducts();
    }, []);

    const renderProduct = ({ item }: { item: Product }) => (
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
                    onPress={() => navigation.navigate('ProductDetails', { productId: item.id })}
                >
                    Details
                </Button>
                <Button
                    mode="contained"
                    onPress={() => cartViewModel.addToCart(item)}
                >
                    Add to Cart
                </Button>
            </Card.Actions>
        </Card>
    );

    return (
        <View style={styles.container}>
            <Searchbar
                placeholder="Search products"
                onChangeText={productViewModel.setSearchQuery}
                value={productViewModel.searchQuery}
                style={styles.searchBar}
            />
            <FlatList
                data={productViewModel.filteredProducts}
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
        backgroundColor: '#f5f5f5',
    },
    searchBar: {
        margin: 16,
        elevation: 4,
    },
    list: {
        padding: 16,
    },
    card: {
        marginBottom: 16,
        elevation: 4,
    },
    cardImage: {
        height: 200,
    },
    price: {
        fontSize: 18,
        fontWeight: 'bold',
        marginTop: 8,
        color: '#007AFF',
    },
});

export default HomeScreen; 