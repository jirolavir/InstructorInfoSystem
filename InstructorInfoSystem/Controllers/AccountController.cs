using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using InstructorInfoSystem.Data;
using InstructorInfoSystem.Models;
using InstructorInfoSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstructorInfoSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<UserAccount> _passwordHasher;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<UserAccount>();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = model.Email.Trim();

            bool emailExists = await _context.UserAccounts
                .AnyAsync(account =>
                    account.Email.ToLower() == email.ToLower());

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            var account = new UserAccount
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Age = model.Age!.Value,
                Address = model.Address.Trim(),
                Institute = model.Institute.Trim(),
                Email = email
            };

            account.PasswordHash = _passwordHasher.HashPassword(
                account, model.Password);

            _context.UserAccounts.Add(account);
            await _context.SaveChangesAsync();

            TempData["Message"] =
                $"Registration successful. Your account ID is {account.UserAccountId}. You may now log in.";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var value = model.EmailOrId.Trim();

            UserAccount? account;

            if (int.TryParse(value, out var accountId))
            {
                account = await _context.UserAccounts
                    .FirstOrDefaultAsync(a => a.UserAccountId == accountId);
            }
            else
            {
                var normalizedEmail = value.ToLowerInvariant();

                account = await _context.UserAccounts
                    .FirstOrDefaultAsync(a =>
                        a.Email.ToLower() == normalizedEmail);
            }

            if (account != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(
                    account,
                    account.PasswordHash,
                    model.Password);

                if (result != PasswordVerificationResult.Failed)
                {

                    account.LastLogin = DateTime.Now;
                    await _context.SaveChangesAsync();

                    var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    account.UserAccountId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    account.Email),

                new Claim(
                    "FirstName",
                    account.FirstName),

                new Claim(
                    "LastName",
                    account.LastName),

                new Claim("Age", account.Age.ToString()),
                new Claim("Address", account.Address),
                new Claim("Institute", account.Institute)

            };

                    var identity = new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal);

                    TempData["Message"] =
                        $"Welcome back, {account.FirstName}!";

                    return RedirectToAction("Index", "Home");
                }
            }

            const string loginError =
                "Login failed. The email address, account ID, or password is incorrect.";

            ModelState.AddModelError(string.Empty, loginError);
            TempData["Message"] = loginError;

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            var accountIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdValue, out var accountId))
            {
                return Forbid();
            }

            var account = await _context.UserAccounts.FindAsync(accountId);

            ViewBag.LastLogin = account.LastLogin;

            if (account == null)
            {
                return NotFound();
            }

            ViewBag.AccountId = account.UserAccountId;
            ViewBag.FirstName = account.FirstName;
            ViewBag.LastName = account.LastName;
            ViewBag.Email = account.Email;
            ViewBag.Age = account.Age;
            ViewBag.Address = account.Address;
            ViewBag.Institute = account.Institute;

            return View();
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var accountIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdValue, out var accountId))
            {
                return Forbid();
            }

            var account = await _context.UserAccounts.FindAsync(accountId);

            var email = model.Email.Trim();

            var emailExists = await _context.UserAccounts
                .AnyAsync(u => u.Email == email && u.UserAccountId != accountId);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            account.FirstName = model.FirstName.Trim();
            account.LastName = model.LastName.Trim();
            account.Email = email;
            account.Age = model.Age!.Value;
            account.Address = model.Address.Trim();
            account.Institute = model.Institute.Trim();

await _context.SaveChangesAsync();

            account.Age = model.Age!.Value;
            account.Address = model.Address.Trim();
            account.Institute = model.Institute.Trim();

            await _context.SaveChangesAsync();
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, account.UserAccountId.ToString()),
    new Claim(ClaimTypes.Name, account.Email),
    new Claim("FirstName", account.FirstName),
    new Claim("LastName", account.LastName),
    new Claim("Age", account.Age.ToString()),
    new Claim("Address", account.Address),
    new Claim("Institute", account.Institute)
};

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            TempData["Message"] = "Your profile was updated successfully.";

            return RedirectToAction(nameof(Profile));
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            var accountIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdValue, out var accountId))
            {
                return Forbid();
            }

            var account = await _context.UserAccounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                FirstName = account.FirstName,
                LastName = account.LastName,
                Email = account.Email,
                Age = account.Age,
                Address = account.Address,
                Institute = account.Institute
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "You have successfully logged out.";

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var accountIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdValue, out var accountId))
            {
                return Forbid();
            }

            var account = await _context.UserAccounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            var passwordHasher = new PasswordHasher<UserAccount>();
            var result = passwordHasher.VerifyHashedPassword(
                account,
                account.PasswordHash,
                model.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    "CurrentPassword",
                    "The current password is incorrect.");

                return View(model);
            }

       

            account.PasswordHash = passwordHasher.HashPassword(
                account,
                model.NewPassword);

            await _context.SaveChangesAsync();

            TempData["Message"] = "Your password was changed successfully.";

            return RedirectToAction(nameof(Profile));
        }


        [HttpGet]
        public IActionResult DeleteAccount()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            return View(new DeleteAccountViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ConfirmationText != "DELETE")
            {
                ModelState.AddModelError(
                    nameof(model.ConfirmationText),
                    "Type DELETE exactly to confirm.");

                return View(model);
            }

            var accountIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdValue, out var accountId))
            {
                return Forbid();
            }

            var account = await _context.UserAccounts.FindAsync(accountId);

            if (account == null)
            {
                return NotFound();
            }

            var passwordHasher = new PasswordHasher<UserAccount>();
            var result = passwordHasher.VerifyHashedPassword(
                account,
                account.PasswordHash,
                model.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    nameof(model.CurrentPassword),
                    "The current password is incorrect.");

                return View(model);
            }

            _context.UserAccounts.Remove(account);
            await _context.SaveChangesAsync();

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "Your account was deleted successfully.";

            return RedirectToAction("Login", "Account");
        }

    }
}
