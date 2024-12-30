import React, { useEffect, useState } from 'react';
import api from '../api';
import AddCategory from './AddCategory';

const Categories = () => {
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const response = await api.get('/Categories');
        setCategories(response.data.data);
      } catch (err) {
        setError(err.message);
        console.error("Error fetching categories:", err);
      }
    };
    fetchCategories();
  }, []);

  const handleCategoryAdded = (newCategory) => {
    setCategories((prevCategories) => [...prevCategories, newCategory]);
  };

  return (
    <div>
      <h1>Categories</h1>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
      <AddCategory onCategoryAdded={handleCategoryAdded} />
      <ul>
        {categories.map(category => (
          <li key={category.id}>{category.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default Categories;
