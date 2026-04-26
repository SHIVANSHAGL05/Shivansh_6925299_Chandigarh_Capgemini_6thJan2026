using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Web.Models;
using BookStore.Web.Services;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Web.Controllers;

public class HomeController : Controller
{
    private readonly IStorefrontService _storefrontService;

    public HomeController(IStorefrontService storefrontService)
    {
        _storefrontService = storefrontService;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Portal));
        }

        return View();
    }

    [Authorize]
    public IActionResult Portal()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin");
        }

        return RedirectToAction("Orders", "Catalog");
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
