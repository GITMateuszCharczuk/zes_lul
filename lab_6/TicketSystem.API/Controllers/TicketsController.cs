using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.API.Hubs;
using TicketSystem.API.Models;
using System.Text.Json;

namespace TicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly IMongoCollection<Ticket> _tickets;
        private readonly IHubContext<TicketHub> _hubContext;

        public TicketsController(IMongoDatabase database, IHubContext<TicketHub> hubContext)
        {
            _tickets = database.GetCollection<Ticket>("Tickets");
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _tickets.Find(_ => true).ToListAsync();
            Console.WriteLine($"Fetched tickets: {tickets.Count}");
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketDto ticketDto)
        {
            Console.WriteLine($"Received ticket: {JsonSerializer.Serialize(ticketDto)}");
            
            if (string.IsNullOrEmpty(ticketDto.Title) || string.IsNullOrEmpty(ticketDto.Description))
            {
                return BadRequest("Title and Description are required");
            }

            var ticket = new Ticket
            {
                Id = Guid.NewGuid().ToString(),
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Open",
                UserId = ticketDto.UserId
            };
            
            await _tickets.InsertOneAsync(ticket);
            await _hubContext.Clients.All.SendAsync("TicketCreated", ticket);
            
            return Ok(ticket);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Ticket ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            await _tickets.ReplaceOneAsync(t => t.Id == id, ticket);
            await _hubContext.Clients.All.SendAsync("TicketUpdated", ticket);
            
            return Ok(ticket);
        }
    }
} 