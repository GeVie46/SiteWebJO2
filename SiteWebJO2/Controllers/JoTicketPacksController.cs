using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SiteWebJO2.Data;
using SiteWebJO2.Models;

namespace SiteWebJO2.Controllers
{
    /// <summary>
    /// class and associated views created with auto generation
    /// </summary>
    [Authorize(Roles = "admin")]
    public class JoTicketPacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JoTicketPacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// display JoTicketPacks, for admin account only
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.JoTicketPacks.ToListAsync());
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
                _context.Add(joTicketPack);
                await _context.SaveChangesAsync();
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

            var joTicketPack = await _context.JoTicketPacks.FindAsync(id);
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
                    _context.Update(joTicketPack);
                    await _context.SaveChangesAsync();
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
            return _context.JoTicketPacks.Any(e => e.JoTicketPackId == id);
        }
    }
}
