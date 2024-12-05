import React from 'react';
import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { TicketList } from './components/TicketList';
import { AdminView } from './components/AdminView';
import { Navbar } from './components/Navbar';

function App() {
  return (
    <Router>
      <div className="App">
        <Navbar />
        <header className="bg-gray-800 text-white py-6 mb-8">
          <h1 className="text-3xl font-bold text-center">Ticket Management System</h1>
        </header>
        <main>
          <Routes>
            <Route path="/" element={<TicketList />} />
            <Route path="/admin" element={<AdminView />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
