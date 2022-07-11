﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Models.ViewModel;
using ParkyWeb.Repository.IRepository;
using System.Diagnostics;
using System.Security.Claims;

namespace ParkyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly INationalParkRepository _npRepo;
        private readonly ITrailRepository _trailRepo;
        private readonly IAccountRepository _accountRepo;

        public HomeController(ILogger<HomeController> logger, 
                              ITrailRepository trailRepo,
                              INationalParkRepository npRepo,
                              IAccountRepository accountRepo)
        {
            _trailRepo = trailRepo;
            _npRepo = npRepo;
            _accountRepo = accountRepo;
        }

        public async Task<IActionResult> Index()
        {
            IndexVM indexVM = new IndexVM();
            var token = HttpContext.Session.GetString("JWToken");
            indexVM.NationalParkList = await _npRepo.GetAllAsync(StaticDetails.NationalParkAPIPath, token);
            indexVM.TrailList = await _trailRepo.GetAllAsync(StaticDetails.TrialAPIPath, token);

            return View(indexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User userPost)
        {
            User userObj = await _accountRepo.LoginAsync(StaticDetails.AccountAPIPath + "authenticate/", userPost);

            if(userObj.Token == null)
            {
                return View();
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, userObj.Username));
            identity.AddClaim(new Claim(ClaimTypes.Role, userObj.Role));

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("JWToken", userObj.Token);

            TempData["alert"] = "Welcome " + userObj.Username;

            return RedirectToAction(nameof(Index));
       }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User userPost)
        {
            bool registerResult = await _accountRepo.RegisterAsync(StaticDetails.AccountAPIPath + "register/", userPost);

            if (!registerResult)
            {
                return View();
            }

            TempData["alert"] = "Registration successful" + userPost.Username;

            return RedirectToAction(nameof(Login));
        }


        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync();

            HttpContext.Session.SetString("JWToken", "");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}