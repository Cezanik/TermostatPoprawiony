using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Termostat.Models;

namespace Termostat.Controllers
{
    [Authorize(Roles = "Współlokator")]
    [Authorize(Roles = "Właściciel")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            
            var allRoles = new List<string> { "Admin", "Właściciel", "Współlokator" };
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUser
            {
                Id = user.Id,
                Email = user.Email,
                Roles = allRoles.Select(role => new RoleCheckbox
                {
                    RoleName = role,
                    IsSelected = userRoles.Contains(role)
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                
                user.Email = model.Email;
                user.UserName = model.Email;
                await _userManager.UpdateAsync(user);

                
                var userRoles = await _userManager.GetRolesAsync(user);
                var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

                
                foreach (var role in userRoles)
                {
                    if (!selectedRoles.Contains(role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                }

                
                foreach (var role in selectedRoles)
                {
                    if (!userRoles.Contains(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }
    }
}