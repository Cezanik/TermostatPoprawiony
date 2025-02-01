using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Termostat.Data;
using Termostat.Models;

namespace Termostat.Controllers
{
    
    [Authorize(Roles = "")]
    public class MieszkanieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MieszkanieController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Wyświetl listę mieszkań użytkownika
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            
            var mieszkanie = await _context.Mieszkania
                .Include(m => m.Wspolokatorzy)
                .ThenInclude(mw => mw.User)
                .FirstOrDefaultAsync(m =>
                    m.UserId == currentUser.Id ||
                    m.Wspolokatorzy.Any(w => w.UserId == currentUser.Id)
                );

            if (mieszkanie == null)
            {
                return RedirectToAction("Create");
            }

           
            bool isWlasciciel = mieszkanie.UserId == currentUser.Id;
            ViewBag.IsWlasciciel = isWlasciciel;

            return View(mieszkanie);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Mieszkanie mieszkanie)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                mieszkanie.UserId = currentUser.Id;

                _context.Add(mieszkanie);
                await _context.SaveChangesAsync();

                
                
                    await _userManager.AddToRoleAsync(currentUser, "Właściciel");
                

                return RedirectToAction(nameof(Index));
            }
            return View(mieszkanie);
        }
        [HttpGet]
        public async Task<IActionResult> AddWspolokator(int mieszkanieId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var mieszkanie1 = await _context.Mieszkania.FindAsync(mieszkanieId);

            
            if (mieszkanie1 == null || mieszkanie1.UserId != currentUser.Id)
            {
                return Forbid(); 
            }
            var mieszkanie = await _context.Mieszkania
                .Include(m => m.Wspolokatorzy)
                .FirstOrDefaultAsync(m => m.Id == mieszkanieId);

            if (mieszkanie == null) return NotFound();

           
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminUserIds = adminUsers.Select(u => u.Id).ToList();

           
            var właściciele = await _context.Mieszkania.Select(m => m.UserId).ToListAsync();
            var współlokatorzy = await _context.MieszkanieWspolokatorzy.Select(mw => mw.UserId).ToListAsync();

            var niedozwoleniUzytkownicy = właściciele
                .Union(współlokatorzy)
                .Union(adminUserIds)
                .Union(new List<string> { mieszkanie.UserId })
                .Distinct()
                .ToList();

           
            var availableUsers = await _userManager.Users
                .Where(u => !niedozwoleniUzytkownicy.Contains(u.Id))
                .ToListAsync();

            var model = new AddWspolokatorViewModel
            {
                MieszkanieId = mieszkanieId,
                AvailableUsers = availableUsers.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddWspolokator(AddWspolokatorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var mieszkanie1 = await _context.Mieszkania.FindAsync(model.MieszkanieId);

                if (mieszkanie1 == null || mieszkanie1.UserId != currentUser.Id)
                {
                    return Forbid();
                }

               
                var mieszkanie = await _context.Mieszkania
                    .Include(m => m.Wspolokatorzy)
                    .FirstOrDefaultAsync(m => m.Id == model.MieszkanieId);

                if (mieszkanie == null) return NotFound();

              
                if (mieszkanie.Wspolokatorzy.Count >= 8)
                {
                    ModelState.AddModelError("", "Maksymalnie można dodać 8 współlokatorów!");
                    return View(model);
                }

                
                var existing = mieszkanie.Wspolokatorzy.Any(mw => mw.UserId == model.SelectedUserId);
                if (existing)
                {
                    ModelState.AddModelError("", "Ten użytkownik jest już współlokatorem!");
                    return View(model);
                }

                var userToAdd = await _userManager.FindByIdAsync(model.SelectedUserId);
                mieszkanie.Wspolokatorzy.Add(new MieszkanieWspolokator
                {
                    MieszkanieId = model.MieszkanieId,
                    UserId = model.SelectedUserId
                });

                
                    await _userManager.AddToRoleAsync(userToAdd, "Współlokator");
                

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> WyslijProsbe()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            
            var usersWithApartments = await _userManager.Users
                .Where(u => u.Id != currentUser.Id && _context.Mieszkania.Any(m => m.UserId == u.Id))
                .ToListAsync();

            var model = new WyslijProsbeViewModel
            {
                DostepniUzytkownicy = usersWithApartments.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.Email
                }).ToList()
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> WyslijProsbe(WyslijProsbeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

              
                var existingRequest = await _context.Zaproszenia
                    .AnyAsync(z => z.NadawcaId == currentUser.Id && z.OdbiorcaId == model.WybranyUzytkownikId);

                if (existingRequest)
                {
                    ModelState.AddModelError("", "Już wysłałeś prośbę do tego użytkownika.");
                    return View(model);
                }

                var zaproszenie = new Zaproszenie
                {
                    NadawcaId = currentUser.Id,
                    OdbiorcaId = model.WybranyUzytkownikId,
                    Status = "Oczekujące"
                };

                _context.Zaproszenia.Add(zaproszenie);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [Authorize]
        public async Task<IActionResult> Powiadomienia()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var zaproszenia = await _context.Zaproszenia
                .Include(z => z.Nadawca)
                .Where(z => z.OdbiorcaId == currentUser.Id && z.Status == "Oczekujące")
                .ToListAsync();

            return View(zaproszenia);
        }
        [HttpPost]
        
        public async Task<IActionResult> UsunPowiadomienie(int id)
        {
            var powiadomienie = await _context.Zaproszenia
                .Include(z => z.Odbiorca)
                .FirstOrDefaultAsync(z => z.Id == id);

            if (powiadomienie == null)
            {
                return NotFound();
            }

            
            var currentUser = await _userManager.GetUserAsync(User);
            if (powiadomienie.OdbiorcaId != currentUser.Id)
            {
                return Forbid();
            }

            _context.Zaproszenia.Remove(powiadomienie);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Powiadomienie zostało usunięte.";
            return RedirectToAction(nameof(Powiadomienia));
        }



    }
}
