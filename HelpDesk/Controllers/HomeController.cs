using HelpDesk.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HelpDesk.Data;
using HelpDesk.Services;
using Microsoft.AspNetCore.Authorization;

namespace HelpDesk.Controllers
{
    public class HomeController : Controller
    {
        private readonly TicketService _ticketService;

        public HomeController(TicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: /Tickets
        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return View(tickets);
        }

        // GET: /Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                await _ticketService.CreateTicketAsync(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: /Tickets/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        // POST: /Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                await _ticketService.UpdateTicketAsync(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // POST: /Tickets/UpdateStatus/5
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _ticketService.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }
    }
}
