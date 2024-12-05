import React, { useEffect, useState } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { CreateTicket } from './CreateTicket';

interface Ticket {
  id: string;
  title: string;
  description: string;
  status: string;
  createdAt: string;
}

export const TicketList = () => {
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [connection, setConnection] = useState<any>(null);

  useEffect(() => {
    // Fetch initial tickets
    fetch('http://localhost:8080/api/tickets')
      .then(res => {
        if (!res.ok) {
          throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();
      })
      .then(data => setTickets(data))
      .catch(error => console.error('Error fetching tickets:', error));

    // Setup SignalR connection
    const newConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:8080/tickethub')
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
    newConnection.start();

    newConnection.on('TicketCreated', (ticket: Ticket) => {
      setTickets(prev => [...prev, ticket]);
    });

    newConnection.on('TicketUpdated', (updatedTicket: Ticket) => {
      setTickets(prev => 
        prev.map(ticket => 
          ticket.id === updatedTicket.id ? updatedTicket : ticket
        )
      );
    });
  }, []);

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="bg-white rounded-lg shadow-lg p-6 mb-8">
        <h2 className="text-2xl font-bold text-gray-800 mb-6">Create New Ticket</h2>
        <CreateTicket />
      </div>
      
      <div className="bg-white rounded-lg shadow-lg p-6">
        <h2 className="text-2xl font-bold text-gray-800 mb-6">Tickets</h2>
        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {tickets.map(ticket => (
            <div key={ticket.id} className="bg-gray-50 rounded-lg p-6 hover:shadow-md transition-shadow">
              <div className="flex justify-between items-start mb-4">
                <h3 className="text-xl font-semibold text-gray-800">{ticket.title}</h3>
                <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                  ticket.status === 'Open' ? 'bg-green-100 text-green-800' :
                  ticket.status === 'InProgress' ? 'bg-yellow-100 text-yellow-800' :
                  'bg-blue-100 text-blue-800'
                }`}>
                  {ticket.status}
                </span>
              </div>
              <p className="text-gray-600 mb-4">{ticket.description}</p>
              <div className="text-sm text-gray-500">
                Created: {new Date(ticket.createdAt).toLocaleDateString()}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}; 