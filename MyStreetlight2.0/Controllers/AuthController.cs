using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MyStreetlight2._0.Data;
using MyStreetlight2._0.DTOs.UserDtos;
using MyStreetlight2._0.Services.UserService;

namespace MyStreetlight2._0.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;

        public AuthController(
            AppDbContext dbContext,
            ILogger<AuthController> logger,
            IUserService userService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto userData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorFeedback"] = $"Enter Valid UserName and Password";
                    return Redirect(Request.Headers["Referer"].ToString());
                }

                //Console.WriteLine(userData.UserName + " " + userData.Password);
                var user = await _userService.GetUserByUserName(userData.UserName);
                //Console.WriteLine(JsonConvert.SerializeObject(user));

                if (user != null)
                {
                    if(await _userService.IsPasswordValid(userData.UserName, userData.Password))
                    {
                        var userPermissions = await _userService.GetUserPermissionsArrayByUserId(user.Id);
                        Console.WriteLine(userPermissions);
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        };

                        foreach (var permission in userPermissions)
                        {
                            claims.Add(new Claim("Permission", permission));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true, // Keep the user logged in across sessions
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30) // Set cookie expiration
                        };

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        TempData["SuccessFeedback"] = "LoggedIn successfully";
                        return RedirectToAction("Index", "Home");
                    }

                    TempData["ErrorFeedback"] = "Invalid Password";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    TempData["ErrorFeedback"] = $"Username: {userData.UserName} does not exist";
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while Login");

                TempData["ErrorFeedback"] = "Error while Login";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessFeedback"] = "Logged out successfully";
            return RedirectToAction("Index", "Auth");
        }
    }
}
