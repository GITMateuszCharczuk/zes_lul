import React, { useState } from 'react';
import api from '../api';

const AddProductDetail = ({ onProductDetailAdded }) => {
  const [productId, setProductId] = useState('');
  const [specifications, setSpecifications] = useState('');
  const [warranty, setWarranty] = useState('');
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    try {
      const response = await api.post('/ProductDetails', {
        productId: parseInt(productId),
        specifications,
        warranty,
      });
      onProductDetailAdded(response.data.data);
      setProductId('');
      setSpecifications('');
      setWarranty('');
    } catch (err) {
      setError(err.message);
      console.error("Error adding product detail:", err);
    }
  };

  return (
    <div>
      <h2>Add Product Detail</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="number"
          value={productId}
          onChange={(e) => setProductId(e.target.value)}
          placeholder="Product ID"
          required
        />
        <input
          type="text"
          value={specifications}
          onChange={(e) => setSpecifications(e.target.value)}
          placeholder="Specifications"
          required
        />
        <input
          type="text"
          value={warranty}
          onChange={(e) => setWarranty(e.target.value)}
          placeholder="Warranty"
          required
        />
        <button type="submit">Add Product Detail</button>
      </form>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
    </div>
  );
};

export default AddProductDetail;