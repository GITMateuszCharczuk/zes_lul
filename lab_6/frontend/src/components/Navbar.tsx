import React from 'react';
import { Link } from 'react-router-dom';

export const Navbar = () => {
  return (
    <nav className="bg-gray-800 text-white py-4">
      <div className="container mx-auto px-4 flex justify-between items-center">
        <div className="flex space-x-4">
          <Link to="/" className="hover:text-gray-300">
            Strona główna
          </Link>
          <Link to="/admin" className="hover:text-gray-300">
            Panel admina
          </Link>
        </div>
      </div>
    </nav>
  );
} 