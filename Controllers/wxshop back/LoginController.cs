using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ztbiyesheji.EntityFramework;

namespace ztbiyesheji.Controllers
{
    public class LoginController : Controller
    {
        private readonly DemoDbContext _context;
        public LoginController(DemoDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        #region---MD5---
        public static string GetMD5(string password)
        {
            string Password = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < s.Length; i++)
            {
                Password = Password + s[i].ToString("X");
            }
            return Password;
        }
        #endregion
        #region---登陆验证---
        public IActionResult Logings(string userName, string userPassWord)
        {
            if (userName == null || userPassWord == null)
            {
                return NotFound();
            }
            try
            {

                if (_context.AdminUser.Where(a => a.AdminUserNumber == userName).Count()!=0)
                {
                    if (_context.AdminUser.Where(a => a.AdminUserPwd == GetMD5(userPassWord)).Count()!=0)
                    {

                        var AId = from a in _context.AdminUser
                                  where a.AdminUserNumber == userName
                                  select a;
                        string Aid = "";
                        foreach (var item in AId)
                        {
                            Aid = item.AdminUserCode;
                        }
                        var bytes = Encoding.UTF8.GetBytes(Aid);
                        HttpContext.Session.Set("Aid", bytes);

                        return Json(Aid);
                    }
                    return Content ("501");
                }
                else
                {
                    return Content("500");
                }
            }
            catch (Exception )
            {
                return Content("404");
            }

        }
    }
    #endregion
}