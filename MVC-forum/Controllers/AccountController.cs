using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_forum.Data;
using MVC_forum.Models;
using MVC_forum.Models.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MVC_forum.Controllers
{
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext) : Controller
    {
        [HttpGet]
         public async Task<IActionResult> Profile(string? username)
         {
             User? user;
             if (username == null)
             {
                 user = await userManager.GetUserAsync(User);
                 if (user == null) return NotFound();

                 user.Articles = await dbContext.Articles.Where(a => a.Author == user.UserName).ToListAsync();

                 var model = new ProfileViewModel
                 {
                     User = user,
                     ChangePassViewModel = new ChangePassViewModel()
                 };
                 return View(model);
             }
             else
             {

                 user = await userManager.FindByNameAsync(username);
                 if (user == null)
                 {
                     TempData["error"] = "User not found.";
                     return RedirectToAction("Index", "Home");
                 }

                 user.Articles = await dbContext.Articles.Where(a => a.Author == user.UserName).ToListAsync();

                 var model = new ProfileViewModel
                 {
                     User = user
                 };
                 return View(model);
             }
         }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel, IFormFile? profilePicture)
        {
            if (!ModelState.IsValid) return View(viewModel);

            if (await userManager.FindByNameAsync(viewModel.Username) != null)
            {
                TempData["error"] = "Uporabniško ime že obstaja!";
                return View(viewModel);
            }

            var user = new User { UserName = viewModel.Username, NormalizedUserName = viewModel.Username.Normalize() };

            if (profilePicture != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pfp", user.Id + Path.GetExtension(profilePicture.FileName));

                using var image = await Image.LoadAsync(profilePicture.OpenReadStream());
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Crop
                }));
                await image.SaveAsync(filePath);

                user.PFPDir = Path.Combine("/pfp", user.Id + Path.GetExtension(profilePicture.FileName));
            }
            else
            {
                user.PFPDir = "/pfp/default-pfp.jpg";
            }

            var result = await userManager.CreateAsync(user, viewModel.Password);

            if (result.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: viewModel.SaveLogin);
                TempData["success"] = "Registracija uspešna!";
                return RedirectToAction("Index", "Home");
            }

            // TODO: fix error handling
            TempData["error"] = result.Errors;
            foreach (var error in ModelState.Values.SelectMany(state => state.Errors))
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.SaveLogin, lockoutOnFailure: false);
            Console.WriteLine(result);

            if (result.Succeeded)
            {
                TempData["success"] = "Prijava uspešna!";
                return RedirectToAction("Index", "Home");
            }

            TempData["error"] = "Napaka pri prijavi!";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfilePicture(IFormFile? profilePicture)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (profilePicture == null) return RedirectToAction("Profile");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pfp", user.Id + Path.GetExtension(profilePicture.FileName));
            var oldProfilePicturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot" + user.PFPDir);

            using var image = await Image.LoadAsync(profilePicture.OpenReadStream());
            image.Mutate(x => x.Resize(new ResizeOptions {
                Size = new Size(200, 200),
                Mode = ResizeMode.Crop
            }));

            if (oldProfilePicturePath != Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pfp/default-pfp.jpg")) System.IO.File.Delete(oldProfilePicturePath);

            await image.SaveAsync(filePath);

            user.PFPDir = Path.Combine("/pfp", user.Id + Path.GetExtension(profilePicture.FileName));
            await userManager.UpdateAsync(user);

            return RedirectToAction("Profile");
        }

        // TODO: fix password change
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            Console.WriteLine(model);
            Console.WriteLine(model.OldPassword);
            Console.WriteLine(model.NewPassword);

            if (user == null) return RedirectToAction("Profile");

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!changePasswordResult.Succeeded) return RedirectToAction("Profile");

            foreach (var error in changePasswordResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            // TODO: add popup messages
            TempData["success"] = "Your password has been changed. Please log in with your new password.";
            return RedirectToAction("Index", "Home");
        }
    }
}
