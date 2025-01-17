import React, { useState } from 'react';
import api from '../api';

const AddTag = ({ onTagAdded }) => {
  const [name, setName] = useState('');
  const [productIds, setProductIds] = useState('');
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    const productIdsArray = productIds.split(',').map(id => parseInt(id.trim())).filter(id => !isNaN(id));

    try {
      const response = await api.post('/Tags', {
        name,
        productIds: productIdsArray,
      });
      onTagAdded(response.data.data);
      setName('');
      setProductIds('');
    } catch (err) {
      setError(err.message);
      console.error("Error adding tag:", err);
    }
  };

  return (
    <div>
      <h2>Add Tag</h2>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          value={name}
          onChange={(e) => setName(e.target.value)}
          placeholder="Tag Name"
          required
        />
        <input
          type="text"
          value={productIds}
          onChange={(e) => setProductIds(e.target.value)}
          placeholder="Product IDs (comma-separated)"
          required
        />
        <button type="submit">Add Tag</button>
      </form>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
    </div>
  );
};

export default AddTag;