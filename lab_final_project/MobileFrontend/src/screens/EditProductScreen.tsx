import React, { useState, useEffect } from 'react';
import { View, StyleSheet, Alert, ScrollView } from 'react-native';
import { TextInput, Button } from 'react-native-paper';
import { observer } from 'mobx-react-lite';
import { useStore } from '../hooks/useStore';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';

type Props = NativeStackScreenProps<any, 'EditProduct'>;

const EditProductScreen: React.FC<Props> = observer(({ route, navigation }) => {
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [price, setPrice] = useState('');
    const [imageUrl, setImageUrl] = useState('');

    const { productViewModel } = useStore();
    const isEditing = route.params?.productId != null;

    useEffect(() => {
        if (isEditing) {
            loadProduct();
        }
    }, []);

    const loadProduct = async () => {
        const product = await productViewModel.loadProduct(route.params.productId);
        if (product) {
            setTitle(product.title);
            setDescription(product.description);
            setPrice(product.price.toString());
            setImageUrl(product.imageUrl || '');
        } else {
            Alert.alert('Error', productViewModel.error || 'Failed to load product');
            navigation.goBack();
        }
    };

    const validateForm = () => {
        if (!title.trim()) {
            Alert.alert('Error', 'Title is required');
            return false;
        }
        if (!description.trim()) {
            Alert.alert('Error', 'Description is required');
            return false;
        }
        if (!price.trim() || isNaN(Number(price)) || Number(price) <= 0) {
            Alert.alert('Error', 'Please enter a valid price');
            return false;
        }
        return true;
    };

    const handleSubmit = async () => {
        if (!validateForm()) return;

        const productData = {
            title: title.trim(),
            description: description.trim(),
            price: Number(price),
            imageUrl: imageUrl.trim() || undefined,
        };

        try {
            if (isEditing) {
                await productViewModel.updateProduct(route.params.productId, productData);
                Alert.alert('Success', 'Product updated successfully');
            } else {
                await productViewModel.createProduct(productData);
                Alert.alert('Success', 'Product created successfully');
            }
            navigation.goBack();
        } catch (error) {
            Alert.alert('Error', productViewModel.error || `Failed to ${isEditing ? 'update' : 'create'} product`);
        }
    };

    return (
        <ScrollView style={styles.container}>
            <TextInput
                label="Title"
                value={title}
                onChangeText={setTitle}
                mode="outlined"
                style={styles.input}
                disabled={productViewModel.isLoading}
            />
            <TextInput
                label="Description"
                value={description}
                onChangeText={setDescription}
                mode="outlined"
                multiline
                numberOfLines={4}
                style={styles.input}
                disabled={productViewModel.isLoading}
            />
            <TextInput
                label="Price"
                value={price}
                onChangeText={setPrice}
                mode="outlined"
                keyboardType="decimal-pad"
                style={styles.input}
                disabled={productViewModel.isLoading}
            />
            <TextInput
                label="Image URL (optional)"
                value={imageUrl}
                onChangeText={setImageUrl}
                mode="outlined"
                style={styles.input}
                disabled={productViewModel.isLoading}
            />
            <Button
                mode="contained"
                onPress={handleSubmit}
                loading={productViewModel.isLoading}
                style={styles.button}
            >
                {isEditing ? 'Update Product' : 'Create Product'}
            </Button>
        </ScrollView>
    );
});

const styles = StyleSheet.create({
    container: {
        flex: 1,
        padding: 16,
        backgroundColor: '#f5f5f5',
    },
    input: {
        marginBottom: 16,
        backgroundColor: 'white',
    },
    button: {
        marginTop: 8,
        padding: 4,
    },
});

export default EditProductScreen; 