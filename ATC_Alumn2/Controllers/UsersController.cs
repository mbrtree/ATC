using ATC_Alumn2.Entities;
using ATC_Alumn2.Models;
using ATC_Alumn2.Services;
using ATCalumni1.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ATC_Alumn2.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly EmailInboxService emailInboxService;

        public UsersController(ApplicationDbContext dbContext, EmailInboxService emailInboxService)
        {
            this.dbContext = dbContext;
            this.emailInboxService = emailInboxService;
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                byte[]? imageBytes = null;
                if (viewModel.ProfileImage != null && viewModel.ProfileImage.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await viewModel.ProfileImage.CopyToAsync(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }

                var user = new Users
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    CurrentJobTitle = viewModel.CurrentJobTitle,
                    Organization = viewModel.Organization,
                    YearOfGraduation = viewModel.YearOfGraduation ?? 0,
                    SemesterOfGraduation = viewModel.SemesterOfGraduation,
                    GitHubProfile = viewModel.GitHubProfile,
                    LinkedInProfile = viewModel.LinkedInProfile,
                    Password = viewModel.Password,
                    ProfileImage = imageBytes,
                    IsAdmin = viewModel.Role == "Admin",
                    IsMentor = viewModel.Role == "Mentor",
                    IsStudent = viewModel.Role == "Student"
                };

                try
                {
                    await dbContext.Users.AddAsync(user);
                    await dbContext.SaveChangesAsync();
                    ModelState.Clear();
                    ViewBag.Message = $"{user.FirstName} {user.LastName} was registered.";
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", $"{user.Email} is already registered.");
                    return View(viewModel);
                }
                return View();
            }
            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var users = await dbContext.Users.ToListAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> MentorsList()
        {
            var mentors = await dbContext.Users
                                          .Where(u => u.IsMentor)
                                          .ToListAsync();
            return View(mentors); 
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new Users
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                IsAdmin = user.IsAdmin,
                IsMentor = user.IsMentor,
                IsStudent = user.IsStudent
            };

            if (user.Password == null)
            {
                ViewBag.Message = "Please enter password.";
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Users viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await dbContext.Users.FindAsync(id);
                if (user != null)
                {
                    user.FirstName = viewModel.FirstName;
                    user.LastName = viewModel.LastName;
                    user.Email = viewModel.Email;
                    user.Password = viewModel.Password;
                    user.PhoneNumber = viewModel.PhoneNumber;
                    user.CurrentJobTitle = viewModel.CurrentJobTitle;
                    user.Organization = viewModel.Organization;
                    user.YearOfGraduation = viewModel.YearOfGraduation;
                    user.SemesterOfGraduation = viewModel.SemesterOfGraduation;
                    user.GitHubProfile = viewModel.GitHubProfile;
                    user.LinkedInProfile = viewModel.LinkedInProfile;
                    user.ProfileImage = viewModel.ProfileImage;
                    user.IsAdmin = viewModel.IsAdmin;
                    user.IsMentor = viewModel.IsMentor;
                    user.IsStudent = viewModel.IsStudent;


                        await dbContext.SaveChangesAsync();
                        ModelState.Clear();

                        ViewBag.Message = $"{user.FirstName} {user.LastName} was updated.";
                    
                    

                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            return View(viewModel);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.Users.FirstOrDefault(x => x.Email == model.Email && x.Password == model.Password);
                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("Name", user.FirstName)
            };

                    // Assign roles based on user properties
                    if (user.IsAdmin)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }
                    if (user.IsMentor)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Mentor"));
                    }
                    if (user.IsStudent)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "Student"));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.Email == HttpContext.User.Identity.Name)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            // Redirect to the Home page after deleting
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult SecurePage()
        {
            ViewBag.Name = HttpContext.User.Identity.Name;
            return View();
        }


        [Authorize]
        public async Task<IActionResult> MyProfile()
        {
            var email = User.Identity.Name; // Get the logged-in user's email
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); // Pass the user data to the view
        }
    }
}