import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import React, { useEffect, useState } from 'react';
import { playNotificationSound } from '../utils/sound';

interface Ticket {
  id: string;
  title: string;
  description: string;
  status: string;
  createdAt: string;
  adminComment?: string;
}

export const AdminView = () => {
  const [tickets, setTickets] = useState<Ticket[]>([]);
  const [editingTicket, setEditingTicket] = useState<Ticket | null>(null);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editStatus, setEditStatus] = useState('');
  const [editAdminComment, setEditAdminComment] = useState('');
  const [hubConnection, setHubConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    const fetchTickets = async () => {
      try {
        const response = await fetch('http://localhost:8080/api/tickets');
        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }
        const data = await response.json();
        setTickets(data);
      } catch (error) {
        console.error('Error fetching initial tickets:', error);
      }
    };

    fetchTickets();
  }, []);

  useEffect(() => {
    const createHubConnection = async () => {
      const connection = new HubConnectionBuilder()
        .withUrl('http://localhost:8080/tickethub')
        .withAutomaticReconnect()
        .build();

      try {
        await connection.start();
        console.log('SignalR Connected in AdminView');

        connection.on('TicketCreated', (ticket: Ticket) => {
          setTickets(prevTickets => [...prevTickets, ticket]);
          playNotificationSound();
        });

        connection.on('TicketUpdated', (updatedTicket: Ticket) => {
          setTickets(prevTickets =>
            prevTickets.map(ticket =>
              ticket.id === updatedTicket.id ? updatedTicket : ticket
            )
          );
        });

        connection.on('TicketDeleted', (ticketId: string) => {
          setTickets(prevTickets =>
            prevTickets.filter(ticket => ticket.id !== ticketId)
          );
        });

        setHubConnection(connection);
      } catch (err) {
        console.error('Error establishing SignalR connection:', err);
      }
    };

    createHubConnection();

    return () => {
      if (hubConnection) {
        hubConnection.stop();
      }
    };
  }, []);

  const handleDelete = async (id: string) => {
    try {
      const response = await fetch(`http://localhost:8080/api/tickets/${id}`, {
        method: 'DELETE',
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      setTickets(tickets.filter(ticket => ticket.id !== id));
    } catch (error) {
      console.error('Error deleting ticket:', error);
    }
  };

  const handleEdit = (ticket: Ticket) => {
    setEditingTicket(ticket);
    setEditTitle(ticket.title);
    setEditDescription(ticket.description);
    setEditStatus(ticket.status);
    setEditAdminComment(ticket.adminComment || '');
  };

  const handleUpdate = async () => {
    if (!editingTicket) return;

    try {
      const response = await fetch(`http://localhost:8080/api/tickets/${editingTicket.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          title: editTitle,
          description: editDescription,
          status: editStatus,
          adminComment: editAdminComment,
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const updatedTicket = await response.json();
      setTickets(tickets.map(ticket => 
        ticket.id === editingTicket.id ? updatedTicket : ticket
      ));
      setEditingTicket(null);
    } catch (error) {
      console.error('Error updating ticket:', error);
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h2 className="text-2xl font-bold text-gray-800 mb-6">Panel Administratora</h2>
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
            {ticket.adminComment && (
              <div className="mb-4 p-2 bg-yellow-50 border border-yellow-200 rounded">
                <p className="text-sm font-medium text-gray-700">Komentarz administratora:</p>
                <p className="text-gray-600">{ticket.adminComment}</p>
              </div>
            )}
            <div className="flex space-x-2">
              <button
                onClick={() => handleEdit(ticket)}
                className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600"
              >
                Edytuj
              </button>
              <button
                onClick={() => handleDelete(ticket.id)}
                className="bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600"
              >
                Usuń
              </button>
            </div>
          </div>
        ))}
      </div>

      {editingTicket && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white p-6 rounded-lg w-full max-w-md">
            <h3 className="text-xl font-bold mb-4">Edytuj bilet</h3>
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Tytuł</label>
                <input
                  type="text"
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  className="w-full border rounded-md p-2"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Opis</label>
                <textarea
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                  className="w-full border rounded-md p-2"
                  rows={3}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Status</label>
                <select
                  value={editStatus}
                  onChange={(e) => setEditStatus(e.target.value)}
                  className="w-full border rounded-md p-2"
                >
                  <option value="Open">Otwarty</option>
                  <option value="InProgress">W trakcie</option>
                  <option value="Closed">Zamknięty</option>
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Komentarz administratora</label>
                <textarea
                  value={editAdminComment}
                  onChange={(e) => setEditAdminComment(e.target.value)}
                  className="w-full border rounded-md p-2"
                  rows={3}
                  placeholder="Dodaj komentarz administratora..."
                />
              </div>
              <div className="flex justify-end space-x-2">
                <button
                  onClick={() => setEditingTicket(null)}
                  className="bg-gray-500 text-white px-4 py-2 rounded-md hover:bg-gray-600"
                >
                  Anuluj
                </button>
                <button
                  onClick={handleUpdate}
                  className="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600"
                >
                  Zapisz
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}; 