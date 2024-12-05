import React from 'react';
import './App.css';
import { TicketList } from './components/TicketList';

function App() {
  return (
    <div className="App">
      <header className="bg-gray-800 text-white py-6 mb-8">
        <h1 className="text-3xl font-bold text-center">Ticket Management System</h1>
      </header>
      <main>
        <TicketList />
      </main>
    </div>
  );
}

export default App;
