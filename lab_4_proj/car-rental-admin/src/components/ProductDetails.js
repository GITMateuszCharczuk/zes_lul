import React, { useEffect, useState } from 'react';
import api from '../api';
import AddProductDetail from './AddProductDetail';

const ProductDetails = () => {
  const [productDetails, setProductDetails] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchProductDetails = async () => {
      try {
        const response = await api.get('/ProductDetails');
        setProductDetails(response.data.data);
      } catch (err) {
        setError(err.message);
        console.error("Error fetching product details:", err);
      }
    };
    fetchProductDetails();
  }, []);

  const handleProductDetailAdded = (newDetail) => {
    setProductDetails((prevDetails) => [...prevDetails, newDetail]);
  };

  return (
    <div>
      <h1>Product Details</h1>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
      <AddProductDetail onProductDetailAdded={handleProductDetailAdded} />
      <ul>
        {productDetails.map(detail => (
          <li key={detail.id}>Product ID: {detail.productId}, Specs: {detail.specifications}</li>
        ))}
      </ul>
    </div>
  );
};

export default ProductDetails;