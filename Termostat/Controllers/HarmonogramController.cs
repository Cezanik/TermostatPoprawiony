using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Termostat.Data;
using Termostat.Models;

namespace Termostat.Controllers
{
    [Authorize]
    public class HarmonogramController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HarmonogramController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

       
        private async Task<Mieszkanie?> GetMieszkanieAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            return await _context.Mieszkania
                .Include(m => m.Wspolokatorzy)
                .FirstOrDefaultAsync(m =>
                    m.UserId == currentUser.Id ||
                    m.Wspolokatorzy.Any(w => w.UserId == currentUser.Id)
                );
        }
        public async Task<IActionResult> Index()
        {
            var mieszkanie = await GetMieszkanieAsync();
            if (mieszkanie == null)
            {
                return RedirectToAction("Create", "Mieszkanie");
            }

            var harmonogramy = await _context.Harmonogram
                .Where(h => h.MieszkanieId == mieszkanie.Id)
                .ToListAsync();

            // Oblicz koszty dla każdego harmonogramu
            var harmonogramyZKosztami = harmonogramy.Select(h => new HarmonogramViewModel
            {
                Harmonogram = h,
                CenaDzienna = ObliczKosztHarmonogramu(h, mieszkanie),
                CenaTygodniowa = ObliczKosztHarmonogramu(h, mieszkanie) * 7,
                CenaMiesieczna = ObliczKosztHarmonogramu(h, mieszkanie) * 30
            }).ToList();

            return View(harmonogramyZKosztami);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Harmonogram harmonogram)
        {
            var mieszkanie = await GetMieszkanieAsync();
            if (mieszkanie == null)
            {
                return RedirectToAction("Create", "Mieszkanie");
            }

            harmonogram.MieszkanieId = mieszkanie.Id;

            if (ModelState.IsValid)
            {
                _context.Harmonogram.Add(harmonogram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(harmonogram);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var harmonogram = await _context.Harmonogram.FindAsync(id);
            if (harmonogram == null)
            {
                return NotFound();
            }

            return View(harmonogram);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Harmonogram harmonogram)
        {
            if (id != harmonogram.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(harmonogram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(harmonogram);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var harmonogram = await _context.Harmonogram.FindAsync(id);
            if (harmonogram == null)
            {
                return NotFound();
            }

            _context.Harmonogram.Remove(harmonogram);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var harmonogram = await _context.Harmonogram.FindAsync(id);
            if (harmonogram == null)
            {
                return NotFound();
            }

            harmonogram.IsActive = !harmonogram.IsActive;
            _context.Update(harmonogram);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private decimal ObliczKosztHarmonogramu(Harmonogram harmonogram, Mieszkanie mieszkanie)
        {
            decimal kurs = 0.1m;
            decimal liczbaOkien = (decimal)mieszkanie.LiczbaOkien;
            decimal liczbaPokoi = (decimal)mieszkanie.LiczbaPokoi;
            decimal bazowaTemperatura = (decimal)mieszkanie.BazowaTemperatura;
            decimal docelowaTemperatura = (decimal)harmonogram.DocelowaTemperatura;
            decimal czasTrwania = (decimal)harmonogram.End - (decimal)harmonogram.Start;

            decimal uruchmienie = Math.Round(
                (liczbaOkien * 1.2m)
                * (liczbaPokoi * 1.5m)
                * ((docelowaTemperatura - bazowaTemperatura) * 0.5m)
                * kurs,
                2,
                MidpointRounding.AwayFromZero
            );

            decimal cenaDzienna = Math.Round(
                uruchmienie * (czasTrwania * 0.5m),
                2,
                MidpointRounding.AwayFromZero
            );

            decimal cenatygodniowa = Math.Round(cenaDzienna * 7, 2, MidpointRounding.AwayFromZero);
            decimal cenamiesieczna = Math.Round(cenaDzienna * 30, 2, MidpointRounding.AwayFromZero);

            return cenaDzienna; 
        }
        public async Task<IActionResult> PodsumowanieKosztow()
        {
            var mieszkanie = await GetMieszkanieAsync();
            if (mieszkanie == null)
            {
                return RedirectToAction("Create", "Mieszkanie");
            }

            // Pobierz aktywne harmonogramy
            var aktywneHarmonogramy = await _context.Harmonogram
                .Where(h => h.MieszkanieId == mieszkanie.Id && h.IsActive)
                .ToListAsync();

            // Oblicz łączne koszty
            decimal lacznyKosztDzienny = 0;
            decimal lacznyKosztTygodniowy = 0;
            decimal lacznyKosztMiesieczny = 0;

            foreach (var harmonogram in aktywneHarmonogramy)
            {
                decimal kosztDzienny = ObliczKosztHarmonogramu(harmonogram, mieszkanie);
                lacznyKosztDzienny += kosztDzienny;
                lacznyKosztTygodniowy += kosztDzienny * 7;
                lacznyKosztMiesieczny += kosztDzienny * 30;
            }

           
            var model = new PodsumowanieKosztowViewModel
            {
                LacznyKosztDzienny = lacznyKosztDzienny,
                LacznyKosztTygodniowy = lacznyKosztTygodniowy,
                LacznyKosztMiesieczny = lacznyKosztMiesieczny
            };

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FiltrujHarmonogramy(
    int? startOd,      
    int? startDo,
    int? endOd,
    int? endDo,
    int? minTemperatura,
    int? maxTemperatura,
    bool? isActive)
        {
            var query = _context.Harmonogram.AsQueryable();

            if (startOd.HasValue)
            {
                query = query.Where(h => h.Start >= startOd.Value);
            }
            if (startDo.HasValue)
            {
                query = query.Where(h => h.Start <= startDo.Value);
            }
            if (endOd.HasValue)
            {
                query = query.Where(h => h.End >= endOd.Value);
            }
            if (endDo.HasValue)
            {
                query = query.Where(h => h.End <= endDo.Value);
            }
           
            if (minTemperatura.HasValue)
            {
                query = query.Where(h => h.DocelowaTemperatura >= minTemperatura.Value);
            }
            if (maxTemperatura.HasValue)
            {
                query = query.Where(h => h.DocelowaTemperatura <= maxTemperatura.Value);
            }

            
            if (isActive.HasValue)
            {
                query = query.Where(h => h.IsActive == isActive.Value);
            }

            var harmonogramy = await query.ToListAsync();
            return View(harmonogramy);
        }
    }

}

  
