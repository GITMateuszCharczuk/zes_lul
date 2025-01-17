import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Home from './components/Home';
import Categories from './components/Categories';
import Product from './components/Product';
import ProductDetails from './components/ProductDetails';
import Tags from './components/Tags';
import Navbar from './components/Navbar';
import './App.css';

const App = () => {
  return (
    <Router>
      <Navbar />
      <div className="container">
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/categories" element={<Categories />} />
          <Route path="/product" element={<Product />} />
          <Route path="/product-details" element={<ProductDetails />} />
          <Route path="/tags" element={<Tags />} />
          {/* Add more routes as needed */}
        </Routes>
      </div>
    </Router>
  );
};

export default App;
