import React from 'react';
import { Link } from 'react-router-dom';

const Home = () => {
  return (
    <div>
      <h1>Welcome to the Car Rental Admin Panel</h1>
      <p>Select a category or product from the menu below:</p>
      <nav>
        <ul>
          <li>
            <Link to="/categories">View Categories</Link>
          </li>
          <li>
            <Link to="/product">View Products</Link>
          </li>
          <li>
            <Link to="/product-details">View Product Details</Link>
          </li>
          <li>
            <Link to="/tags">View Tags</Link>
          </li>
        </ul>
      </nav>
    </div>
  );
};

export default Home;
