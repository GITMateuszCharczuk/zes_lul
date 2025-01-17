import React, { useEffect, useState } from 'react';
import api from '../api';
import AddTag from './AddTag';

const Tags = () => {
  const [tags, setTags] = useState([]);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchTags = async () => {
      try {
        const response = await api.get('/Tags');
        setTags(response.data.data);
      } catch (err) {
        setError(err.message);
        console.error("Error fetching tags:", err);
      }
    };
    fetchTags();
  }, []);

  const handleTagAdded = (newTag) => {
    setTags((prevTags) => [...prevTags, newTag]);
  };

  return (
    <div>
      <h1>Tags</h1>
      {error && <p style={{ color: 'red' }}>Error: {error}</p>}
      <AddTag onTagAdded={handleTagAdded} />
      <ul>
        {tags.map(tag => (
          <li key={tag.id}>{tag.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default Tags;