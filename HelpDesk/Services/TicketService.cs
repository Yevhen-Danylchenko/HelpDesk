using HelpDesk.Data;
using Microsoft.Extensions.Caching.Distributed;
using HelpDesk.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Services
{
    public class TicketService
    {
        private readonly HelpDeskDbContext _db;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "tickets_all";

        public TicketService(HelpDeskDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            var cachedData = await _cache.GetStringAsync(CacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return System.Text.Json.JsonSerializer.Deserialize<List<Ticket>>(cachedData) ?? new List<Ticket>();
            }
            var tickets = await _db.Tickets.ToListAsync();
            var serializedData = System.Text.Json.JsonSerializer.Serialize(tickets);
            await _cache.SetStringAsync(CacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return tickets;
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync();
            await InvalidateCacheAsync();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var existingTicket = await _db.Tickets.FindAsync(ticket.Id);
            if (existingTicket != null) { 
                existingTicket.Title = ticket.Title;
                existingTicket.Description = ticket.Description;
                existingTicket.Status = ticket.Status;
                existingTicket.AuthorId = ticket.AuthorId;
                existingTicket.AuthorName = ticket.AuthorName;
                await _db.SaveChangesAsync();
                await InvalidateCacheAsync();                
            }
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var existingTicket = await _db.Tickets.FindAsync(id);
            if (existingTicket != null)
            {
                existingTicket.Status = status;
                await _db.SaveChangesAsync();
                await InvalidateCacheAsync();
            }
        }

        public async Task InvalidateCacheAsync()
        {
            await _cache.RemoveAsync(CacheKey);
        }   
    }
}
