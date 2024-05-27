using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ApplicationDbContext _applicationDbContext;

        public JoTicketPacksController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        /// <summary>
        /// display JoTicketPacks, for admin account only
        /// </summary>
        /// <returns>view of all existing packs</returns>
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

            // gather last results together
            var result = (from pack in packList
                         join ticket in ticketList on pack.JoTicketPackId equals ticket.JoTicketPackId into groupPack
                         from subgroup in groupPack.DefaultIfEmpty()
                         select new DisplayedJoTicketPack { JoTicketPackId = pack.JoTicketPackId, JoTicketPackName = pack.JoTicketPackName,
                             NbAttendees = pack.NbAttendees, ReductionRate = pack.ReductionRate, JoTicketPackStatus = pack.JoTicketPackStatus,
                             NbPacksSold = subgroup?.Count ?? 0}).ToList();

            return View(result.OrderByDescending(p => p.JoTicketPackStatus).ThenByDescending(p=>p.NbPacksSold));
        }


        /// <summary>
        /// view to create a new JoTicketPack, for admin account only
        /// </summary>
        /// <returns>view to create a new JoTicketPack</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// create a new pack
        /// </summary>
        /// <param name="joTicketPack">data of pack</param>
        /// <returns>view that display the new pack</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JoTicketPackId,JoTicketPackName,NbAttendees,ReductionRate,JoTicketPackStatus")] JoTicketPack joTicketPack)
        {
            var pack = new JoTicketPack { };
            if (ModelState.IsValid)
            {
                // change value of reduction rate : /100
                pack = new JoTicketPack
                {
                    JoTicketPackId = joTicketPack.JoTicketPackId,
                    JoTicketPackName = joTicketPack.JoTicketPackName,
                    NbAttendees = joTicketPack.NbAttendees,
                    ReductionRate = joTicketPack.ReductionRate / 100,
                    JoTicketPackStatus = joTicketPack.JoTicketPackStatus
                };

                _applicationDbContext.Add(joTicketPack);
                await _applicationDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pack);
        }

        /// <summary>
        /// display data of pack selected
        /// </summary>
        /// <param name="id">id of the pack chosen</param>
        /// <returns>view of the selected pack</returns>
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
            // change display of reduction rate : *100
            var pack = new JoTicketPack {
                JoTicketPackId = joTicketPack.JoTicketPackId,
                JoTicketPackName = joTicketPack.JoTicketPackName,
                NbAttendees = joTicketPack.NbAttendees,
                ReductionRate = joTicketPack.ReductionRate * 100,
                JoTicketPackStatus = joTicketPack.JoTicketPackStatus
            };
            return View(pack);
        }

        /// <summary>
        /// change data of a pack
        /// </summary>
        /// <param name="id">id of pack to change</param>
        /// <param name="joTicketPack">data of pack</param>
        /// <returns>view of the changed pack</returns>
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
                    // change value of reduction rate : /100
                    var pack = new JoTicketPack
                    {
                        JoTicketPackId = joTicketPack.JoTicketPackId,
                        JoTicketPackName = joTicketPack.JoTicketPackName,
                        NbAttendees = joTicketPack.NbAttendees,
                        ReductionRate = joTicketPack.ReductionRate / 100,
                        JoTicketPackStatus = joTicketPack.JoTicketPackStatus
                    };

                    _applicationDbContext.Update(pack);
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

       /// <summary>
       /// function to check if a pack exists
       /// </summary>
       /// <param name="id">id of the pack to check</param>
       /// <returns>boolean to say if the pack exists</returns>
        private bool JoTicketPackExists(int id)
        {
            return _applicationDbContext.JoTicketPacks.Any(e => e.JoTicketPackId == id);
        }
    }
}
