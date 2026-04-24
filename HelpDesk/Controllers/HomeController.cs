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

        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return View(tickets);
        }

        public IActionResult Create()
        {
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            var ticket = tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }


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

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _ticketService.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(Index));
        }
    }
}
