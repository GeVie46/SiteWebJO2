using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using SiteWebJO2.Data;
using SiteWebJO2.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SiteWebJO2.Controllers
{
    /// <summary>
    /// class and associated views created with auto generation
    /// </summary>
    [Authorize(Roles = "admin")]
    public class JoTicketPacksController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public JoTicketPacksController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        /// <summary>
        /// display JoTicketPacks, for admin account only
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            // get JoTicketPack data
            List<JoTicketPack> packList = (from p in _applicationDbContext.JoTicketPacks
                      select p).ToList();

            // get nb of pack sold
            var ticketList = (from t in _applicationDbContext.JoTickets
                              select t).ToList().GroupBy(JoTicket => JoTicket.JoTicketPackId)
                                   .Select(group => new
                                   {
                                       JoTicketPackId = group.Key,
                                       Count = group.Count()
                                   });
            var result = (from pack in packList
                         join ticket in ticketList on pack.JoTicketPackId equals ticket.JoTicketPackId
                         select new DisplayedJoTicketPack { JoTicketPackId = pack.JoTicketPackId, JoTicketPackName = pack.JoTicketPackName,
                             NbAttendees = pack.NbAttendees, ReductionRate = pack.ReductionRate, JoTicketPackStatus = pack.JoTicketPackStatus,
                             NbPacksSold = ticket.Count }).ToList();

            return View(result.OrderByDescending(p => p.JoTicketPackStatus).ThenByDescending(p=>p.NbPacksSold));
        }


        /// <summary>
        /// view to create a new JoTicketPack, for admin account only
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        // POST: JoTicketPacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JoTicketPackId,JoTicketPackName,NbAttendees,ReductionRate,JoTicketPackStatus")] JoTicketPack joTicketPack)
        {
            if (ModelState.IsValid)
            {
                _applicationDbContext.Add(joTicketPack);
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joTicketPack);
        }

        // GET: JoTicketPacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joTicketPack = await _applicationDbContext.JoTicketPacks.FindAsync(id);
            if (joTicketPack == null)
            {
                return NotFound();
            }
            return View(joTicketPack);
        }

        // POST: JoTicketPacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JoTicketPackId,JoTicketPackName,NbAttendees,ReductionRate,JoTicketPackStatus")] JoTicketPack joTicketPack)
        {
            if (id != joTicketPack.JoTicketPackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _applicationDbContext.Update(joTicketPack);
                    await _applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JoTicketPackExists(joTicketPack.JoTicketPackId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(joTicketPack);
        }

       
        private bool JoTicketPackExists(int id)
        {
            return _applicationDbContext.JoTicketPacks.Any(e => e.JoTicketPackId == id);
        }
    }
}
