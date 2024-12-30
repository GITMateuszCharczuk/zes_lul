import React, { useEffect, useState } from 'react';
import api from '../api';
import AddProduct from './AddProduct';

const Products = () => {
  const [products, setProducts] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await api.get('/Products');
        setProducts(response.data.data);
      } catch (err) {
        setError(err.message);
        console.error("Error fetching products:", err);
      }
    };
    fetchProducts();
  }, []);

  const handleProductAdded = (newProduct) => {
    setProducts((prevProducts) => [...prevProducts, newProduct]);
  };

  return (
    <div>
      <h1>Products</h1>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
      <AddProduct onProductAdded={handleProductAdded} />
      <ul>
        {products.map(product => (
          <li key={product.id}>
            <strong>Title:</strong> {product.title} <br />
            <strong>Description:</strong> {product.description} <br />
            <strong>Barcode:</strong> {product.barcode} <br />
            <strong>Price:</strong> ${product.price.toFixed(2)} <br />
            <strong>Release Date:</strong> {new Date(product.releaseDate).toLocaleDateString()} <br />
          </li>
        ))}
      </ul>
    </div>
  );
};

export default Products;