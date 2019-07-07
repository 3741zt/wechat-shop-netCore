using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ContosoUniversity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ztbiyesheji.Controllers;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;
using static ztbiyesheji.Models.ENUM.Enum;

namespace ztbiyesheji.Views
{
    public class AdminUsersController : Controller
    {

        private readonly DemoDbContext _context;

        public AdminUsersController(DemoDbContext context)
        {
            _context = context;
        }

        // 管理员列表
        public async Task<IActionResult> AdminUsersIndex(string search, string sortOrder, string currentFilter, int? pageNumber,int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var Total = _context.AdminUser.ToList().Count();
            ViewBag.Total = Total;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = search;
            var a = ViewData["CurrentFilter"];
            if (search == null && a == null && state == null)
            {
                pageNumber = 1;
                currentFilter = null;
            }
            if (search != null)
            {
                pageNumber = 1;
            }
            else
            {
                search = currentFilter;
            }
            var AdminUsers = from s in _context.AdminUser
                             select s;
            if (!String.IsNullOrEmpty(search))
            {
                AdminUsers = AdminUsers.Where(s => s.AdminUserName.Contains(search)
                                       || s.AdminUserNumber.Contains(search)||s.RoleType.ToString()==search|| s.AdminUserSex.ToString() == search);
            }
            switch (sortOrder)
            {
                case "AdminUserNumber":
                    AdminUsers = AdminUsers.OrderByDescending(s => s.AdminUserNumber);
                    break;
                case "AdminUserName":
                    AdminUsers = AdminUsers.OrderBy(s => s.AdminUserName);
                    break;
            }
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = AdminUsers.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            if (pageNumber > Totalpage)
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<AdminUser>.CreateAsync(AdminUsers.AsNoTracking(), pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0)
            {
                return View(await PaginatedList<AdminUser>.CreateAsync(AdminUsers.AsNoTracking(), 1, pageSize));
            }
            return View(await PaginatedList<AdminUser>.CreateAsync(AdminUsers.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // 管理员详情
        public async Task<IActionResult> AdminUsersDetails(string Id)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Id == null)
            {
                return NotFound();
            }

            var adminUser = await _context.AdminUser
                .SingleOrDefaultAsync(m => m.AdminUserCode == Id);
            if (adminUser == null)
            {
                return NotFound();
            }

            return View(adminUser);
        }
        //修改
        public IActionResult CheckAdminUsers(AdminUserSexes AdminUserSex, AdminUserRoleType RoleType, string AdminUserName, string AdminUserNumber, string AdminUserPhoneNumber, string AdminUserPwd,string Code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (RoleType.ToString() == null || AdminUserName == null || AdminUserNumber == null|| AdminUserPwd==null)
            {
                return Content("404");
            }
            if (_context.AdminUser.Where(a => a.AdminUserNumber == AdminUserNumber).Count() == 0 || _context.AdminUser.Where(a => a.AdminUserName == AdminUserName).Count() == 0 || _context.AdminUser.Where(a => a.AdminUserPhoneNumber == AdminUserPhoneNumber).Count() == 0)
            {
                try
                {
                    var UpdateAdminUsers = _context.AdminUser.Where(a => a.AdminUserCode == Code).FirstOrDefault();
                    UpdateAdminUsers.AdminUserSex = AdminUserSex;
                    UpdateAdminUsers.RoleType = RoleType;
                    UpdateAdminUsers.AdminUserName = AdminUserName;
                    UpdateAdminUsers.AdminUserNumber = AdminUserNumber;
                    UpdateAdminUsers.AdminUserPhoneNumber = AdminUserPhoneNumber;
                    UpdateAdminUsers.AdminUserPwd = AdminUserPwd;
                    _context.SaveChanges();
                    return Content("200");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Content("500");
                }
            }
            else
            {
                return Content("301");
            }
        }

        // 添加
        public IActionResult AdminUsersCreate()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        //添加动作
        public async Task<IActionResult> AdminUsersCreates(AdminUserSexes AdminUserSex, AdminUserRoleType RoleType, string AdminUserName, string AdminUserNumber, string AdminUserPhoneNumber, string AdminUserPwd)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (RoleType.ToString()==""|| AdminUserName==""|| AdminUserNumber==""|| AdminUserPwd == "")
            {
                return Content("400");
            }
            if (_context.AdminUser.Where(a => a.AdminUserNumber == AdminUserNumber).Count() == 0 || _context.AdminUser.Where(a => a.AdminUserName == AdminUserName).Count() == 0 || _context.AdminUser.Where(a => a.AdminUserPhoneNumber == AdminUserPhoneNumber).Count() == 0)
            {
                string AdminUserMD5Pwd = "";
                MD5 md5 = MD5.Create();
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(AdminUserPwd));
                for (int i = 0; i < s.Length; i++)
                {
                    AdminUserMD5Pwd = AdminUserMD5Pwd + s[i].ToString("X");
                }
                try
                {

                    var AdminUser = new AdminUser
                    {
                        AdminUserCode = AdminUserNumber+(int)RoleType,
                        AdminUserName = AdminUserName,
                        AdminUserNumber = AdminUserNumber,
                        AdminUserPwd = AdminUserMD5Pwd,
                        RoleType = RoleType,
                        AdminUserSex = AdminUserSex,
                        AdminUserPhoneNumber = AdminUserPhoneNumber
                    };
                    _context.Add(AdminUser);
                    await _context.SaveChangesAsync();
                    return Content("200");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Content("500");
                }
            }
            else
            {
                return Content("201");
            }
        }


        // 删除
        public IActionResult DeleteAdminUsers(string code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (code == null)
            {
                return Content("500");
            }
            AdminUser AdminUser = _context.AdminUser.FirstOrDefault(u => u.AdminUserCode == code);
            _context.AdminUser.Remove(AdminUser);
            _context.SaveChanges();
            return Content("200");
        }

    }
}
