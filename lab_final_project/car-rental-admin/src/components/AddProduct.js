import React, { useState } from 'react';
import api from '../api';

const AddProduct = ({ onProductAdded }) => {
  const [barcode, setBarcode] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('');
  const [title, setTitle] = useState('');
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    try {
      const response = await api.post('/Products', {
        barcode,
        description,
        price: parseFloat(price),
        title,
      });
      onProductAdded(response.data.data);
      setBarcode('');
      setDescription('');
      setPrice('');
      setTitle('');
    } catch (err) {
      setError(err.message);
      console.error("Error adding product:", err);
    }
  };

  return (
    <div>
      <h2>Add Product</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={barcode}
          onChange={(e) => setBarcode(e.target.value)}
          placeholder="Barcode"
          required
        />
        <input
          type="text"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          placeholder="Description"
          required
        />
        <input
          type="number"
          value={price}
          onChange={(e) => setPrice(e.target.value)}
          placeholder="Price"
          required
        />
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Title"
          required
        />
        <button type="submit">Add Product</button>
      </form>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
    </div>
  );
};

export default AddProduct;