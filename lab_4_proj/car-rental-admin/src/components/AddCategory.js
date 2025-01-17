import React, { useState } from 'react';
import api from '../api';

const AddCategory = ({ onCategoryAdded }) => {
  const [name, setName] = useState('');
  const [productIds, setProductIds] = useState('');
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    const productIdsArray = productIds.split(',').map(id => parseInt(id.trim())).filter(id => !isNaN(id));

    try {
      const response = await api.post('/Categories', {
        name,
        productIds: productIdsArray,
      });
      onCategoryAdded(response.data.data);
      setName('');
      setProductIds('');
    } catch (err) {
      setError(err.message);
      console.error("Error adding category:", err);
    }
  };

  return (
    <div>
      <h2>Add Category</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Category Name"
          required
        />
        <input
          type="text"
          value={productIds}
          onChange={(e) => setProductIds(e.target.value)}
          placeholder="Product IDs (comma-separated)"
          required
        />
        <button type="submit">Add Category</button>
      </form>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
    </div>
  );
};

export default AddCategory;