using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;

namespace ztbiyesheji.Controllers
{
    public class HomeController : Controller
    {
        private readonly DemoDbContext _context;
        public HomeController(DemoDbContext context)
        {
            _context = context;
        }
   
        public async Task<IActionResult> HomeAsync(string Id)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var AdminUser = await _context.AdminUser
                .SingleOrDefaultAsync(m => m.AdminUserCode == Id);
            return View(AdminUser);
        }
        public IActionResult Welcome()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult RemoveSeeion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
    }
}
