using Microsoft.AspNetCore.Mvc;
using SiteWebJO2.Models;
using SiteWebJO2.Data;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;

namespace SiteWebJO2.Controllers
{
    public class JoSessionsController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        //constructor, with dependency injection of dbContext
        public JoSessionsController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;

            if (String.IsNullOrEmpty(sortOrder)){ sortOrder = "date_asc"; }
            ViewData["NameSortParm"] = sortOrder == "name_desc" ? "name_asc" : "name_desc";
            ViewData["PlaceSortParm"] = sortOrder == "place_desc" ? "place_asc" : "place_desc";
            ViewData["DateSortParm"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            // get data
            var js = from s in _applicationDbContext.JoSessions
                     select s;
            js = GetAvailableJoSessions(js);

            // filtering data
            if (!String.IsNullOrEmpty(searchString))
            {
                js = js.Where(s => s.JoSessionName.Contains(searchString)
                                       || s.JoSessionPlace.Contains(searchString)
                                        );
            }

            // sorting data
            switch (sortOrder)
            {
                case "name_desc":
                    js = js.OrderByDescending(s => s.JoSessionName);
                    break;
                case "name_asc":
                    js = js.OrderBy(s => s.JoSessionName);
                    break;
                case "place_desc":
                    js = js.OrderByDescending(s => s.JoSessionPlace);
                    break;
                case "place_asc":
                    js = js.OrderBy(s => s.JoSessionPlace);
                    break;
                case "":
                default:
                    js = js.OrderBy(s => s.JoSessionDate);
                    break;
                case "date_desc":
                    js = js.OrderByDescending(s => s.JoSessionDate);
                    break;
            }
            int pageSize = 10;

            return View(await PaginatedList<JoSession>.CreateAsync(js.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [Route("JoSessions/Details/{joSessionId}")]
        public async Task<IActionResult> Details(int? joSessionId)
        {
            if (joSessionId == null || _applicationDbContext.JoSessions == null)
            {
                return NotFound();
            }

            var joSession = await _applicationDbContext.JoSessions.FirstOrDefaultAsync(s => s.JoSessionId == joSessionId);
            if (joSession == null) 
            { 
                return NotFound();
            }
            
            return View(joSession);
        }


        /*
         * get available JoSessions
         */
        public static IQueryable<JoSession> GetAvailableJoSessions(IQueryable<JoSession> joSessionsList)
        {
            return joSessionsList.Where(s => s.JoSessionNbTotalBooked < s.JoSessionNbTotalAttendees);
        }


        /* 
         * création d'une liste de JoSession
         * ajout des données à la base de données
         * à appeler à la main
         */
        public async Task<IActionResult> AddData()
        {
            // JoSessionId auto généré
            List<JoSession> JoSessionsList = new List<JoSession>
                {
                new JoSession {JoSessionName = "Equestrian Dressage - Mixed", JoSessionDate = new DateTime(2024, 8, 1, 11, 0, 0), JoSessionPlace = "Versailles, Château de Versailles", JoSessionNbTotalAttendees= 10000, JoSessionNbTotalBooked= 5000, JoSessionDescription="EQD01 Dressage grand prix team and individual qualifier", JoSessionImage=@"~/images/Equestrian.jpg", JoSessionPrice=50 },
                new JoSession {JoSessionName = "Artistic swimming - Female", JoSessionDate = new DateTime(2024, 8, 10, 19, 30, 0), JoSessionPlace = "St Denis, Aquatics centre", JoSessionNbTotalAttendees= 8000, JoSessionNbTotalBooked= 8000, JoSessionDescription="SWA05 Duet free routine", JoSessionImage= @"~/images/Artistic Swimming.jpg", JoSessionPrice=80 },
                new JoSession {JoSessionName = "Rowing - Mixed", JoSessionDate = new DateTime(2024, 7, 29, 9, 30, 0), JoSessionPlace = "Vaires-sur-Marne, Nautical St. - Flatwater", JoSessionNbTotalAttendees= 15000, JoSessionNbTotalBooked= 6000, JoSessionDescription="ROW01 M/W - Heats\r\n\r\nScheduled events (subject to change):\r\nMen's Single Sculls Heats\r\nWomen's Single Sculls Heats\r\nMen's Double Sculls Heats\r\nWomen's Double Sculls Heats\r\nMen's Quadruple Sculls Heats\r\nWomen's Quadruple Sculls Heats", JoSessionImage = @"~/images/Rowing.jpg", JoSessionPrice=30 },
                new JoSession {JoSessionName = "Volleyball - Male", JoSessionDate = new DateTime(2024, 7, 31, 9, 0, 0), JoSessionPlace = "Paris, South Paris Arena 1", JoSessionNbTotalAttendees= 5000, JoSessionNbTotalBooked= 4000, JoSessionDescription="VVO16 M - Preliminary round", JoSessionImage = @"~/images/Volleyball.jpg", JoSessionPrice=90 },
                new JoSession {JoSessionName = "Cycling Moutain Bike - Female", JoSessionDate = new DateTime(2024, 7, 28, 14, 0, 0), JoSessionPlace = "St-Quentin-En-Yvelines, Elancourt Hill", JoSessionNbTotalAttendees= 20000, JoSessionNbTotalBooked= 12000, JoSessionDescription="", JoSessionImage = @"~/images/Cycling Moutain Bike.jpg", JoSessionPrice=70 },
                new JoSession {JoSessionName = "Football - Male", JoSessionDate = new DateTime(2024, 7, 24, 15, 0, 0), JoSessionPlace = "Paris, Parc des Princes", JoSessionNbTotalAttendees= 10000, JoSessionNbTotalBooked= 5000, JoSessionDescription="FBL01 M - AFC2 vs Spain group stage", JoSessionImage=@"~/images/Football.jpg", JoSessionPrice=50 },
                new JoSession {JoSessionName = "Football - Female", JoSessionDate = new DateTime(2024, 7, 25, 17, 00, 0), JoSessionPlace = "Nantes, La Beaujoire Stadium", JoSessionNbTotalAttendees= 8000, JoSessionNbTotalBooked= 8000, JoSessionDescription="FBL03 M - Egypt vs Dominican Republic group stage", JoSessionImage= @"~/images/Football.jpg", JoSessionPrice=30 },
                new JoSession {JoSessionName = "Tennis - Mixed", JoSessionDate = new DateTime(2024, 7, 28, 12, 00, 0), JoSessionPlace = "Paris, Roland-Garros Stadium (Philippe-Chatrier)", JoSessionNbTotalAttendees= 15000, JoSessionNbTotalBooked= 6000, JoSessionDescription="TEN15 M/W - Singles", JoSessionImage = @"~/images/Tennis.jpg", JoSessionPrice=30 },
                new JoSession {JoSessionName = "Basketball - Female", JoSessionDate = new DateTime(2024, 7, 28, 9, 0, 0), JoSessionPlace = "Lille Metropole, Pierre Mauroy Stadium", JoSessionNbTotalAttendees= 5000, JoSessionNbTotalBooked= 4000, JoSessionDescription="BKB08 W - Group phase (1 match): Canada - France", JoSessionImage = @"~/images/Basketball.jpg", JoSessionPrice=90 },
                new JoSession {JoSessionName = "Athletics - Mixed", JoSessionDate = new DateTime(2024, 8, 2, 10, 0, 0), JoSessionPlace = "Saint-Denis, Stade de France", JoSessionNbTotalAttendees= 20000, JoSessionNbTotalBooked= 12000, JoSessionDescription="ATH04 M/W/Mixed - Repechages, semi-finals, finals", JoSessionImage = @"~/images/Athletics.jpg", JoSessionPrice=70 },
                new JoSession {JoSessionName = "Diving - Male", JoSessionDate = new DateTime(2024, 8, 2, 11, 0, 0), JoSessionPlace = "Saint Denis, Aquatics centre", JoSessionNbTotalAttendees= 10000, JoSessionNbTotalBooked= 5000, JoSessionDescription="DIV03 W - Synchronised 10m platform", JoSessionImage=@"~/images/Diving.jpg", JoSessionPrice=50 },
                new JoSession {JoSessionName = "Sailing - Mixed", JoSessionDate = new DateTime(2024, 8, 2, 12, 00, 0), JoSessionPlace = "Marseille, Marseille Marina", JoSessionNbTotalAttendees= 8000, JoSessionNbTotalBooked= 8000, JoSessionDescription="SAL03 M/W - Windsurfing, skiff - opening series", JoSessionImage= @"~/images/Sailing.jpg", JoSessionPrice=80 },
                new JoSession {JoSessionName = "Swimming - Mixed", JoSessionDate = new DateTime(2024, 8, 2, 11, 00, 0), JoSessionPlace = "Nanterre, Paris La Defense Arena", JoSessionNbTotalAttendees= 15000, JoSessionNbTotalBooked= 6000, JoSessionDescription="SWM06 M/W - Semi-finals, finals\r\n\r\nScheduled events (subject to change):\r\nWomen's 400m Medley - Final\r\nMen's 200m Freestyle - Final\r\nWomen's 100m Backstroke - Semi-finals\r\nMen's 100m Backstroke - Final\r\nWomen's 100m Breaststroke - Final\r\nWomen's 200m Freestyle - Final", JoSessionImage = @"~/images/Swimming.jpg", JoSessionPrice=30 },
                new JoSession {JoSessionName = "Badminton - Mixed", JoSessionDate = new DateTime(2024, 8, 2, 15, 0, 0), JoSessionPlace = "Paris, Porte de La Chapelle Arena", JoSessionNbTotalAttendees= 5000, JoSessionNbTotalBooked= 4000, JoSessionDescription="BDM20 M - Singles: 1/4 finals / mixed doubles: bronze, final", JoSessionImage = @"~/images/Badminton.jpg", JoSessionPrice=90 },
                new JoSession {JoSessionName = "Football - Male", JoSessionDate = new DateTime(2024, 8, 2, 17, 0, 0), JoSessionPlace = "Lyon, Lyon Stadium", JoSessionNbTotalAttendees= 20000, JoSessionNbTotalBooked= 12000, JoSessionDescription="FBL04 M - ICP AFC - CAF vs New Zealand group stage", JoSessionImage = @"~/images/Football.jpg", JoSessionPrice=70 },
                new JoSession {JoSessionName = "Handball - Female", JoSessionDate = new DateTime(2024, 8, 3, 19, 0, 0), JoSessionPlace = "Paris, South Paris Arena 6", JoSessionNbTotalAttendees= 10000, JoSessionNbTotalBooked= 5000, JoSessionDescription="HBL27 W - Preliminaries (2 matches)", JoSessionImage=@"~/images/Handball.jpg", JoSessionPrice=50 },
                new JoSession {JoSessionName = "Artistic Gymnastics - Male", JoSessionDate = new DateTime(2024, 7, 29, 17, 30, 0), JoSessionPlace = "Paris, Bercy Arena", JoSessionNbTotalAttendees= 8000, JoSessionNbTotalBooked= 8000, JoSessionDescription="GAR08 M - Team final", JoSessionImage= @"~/images/Artistic Gymnastics.jpg", JoSessionPrice=80 },
            };

            _applicationDbContext.JoSessions.AddRange(JoSessionsList);
            await _applicationDbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

            
}
}
