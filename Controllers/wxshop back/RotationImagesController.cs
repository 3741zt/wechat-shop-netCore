using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;

namespace ztbiyesheji.Views
{
    public class RotationImagesController : Controller
    {
        private readonly DemoDbContext _context;

        public RotationImagesController(DemoDbContext context)
        {
            _context = context;
        }

        // 列表
        public async Task<IActionResult> RotationImagesIndex(string search, string sortOrder, string currentFilter, int? pageNumber, int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var Total = _context.RotationImage.ToList().Count();
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
            var RotationImage = from s in _context.RotationImage
                                select s;
            if (!String.IsNullOrEmpty(search))
            {
                RotationImage = RotationImage.Where(s => s.RotationImageJumpUrl.Contains(search)
                                       || s.RotationImageUrl.Contains(search));
            }
            switch (sortOrder)
            {
                case "RotationImageJumpUrl":
                    RotationImage = RotationImage.OrderByDescending(s => s.RotationImageJumpUrl);
                    break;
                case "RotationImageUrl":
                    RotationImage = RotationImage.OrderBy(s => s.RotationImageUrl);
                    break;
            }
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = RotationImage.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            if (pageNumber > Totalpage)
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<RotationImage>.CreateAsync(RotationImage.AsNoTracking(), pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0)
            {
                return View(await PaginatedList<RotationImage>.CreateAsync(RotationImage.AsNoTracking(), 1, pageSize));
            }
            return View(await PaginatedList<RotationImage>.CreateAsync(RotationImage.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // 详情/修改
        public async Task<IActionResult> RotationImagesDetails(int? id)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (id == null)
            {
                return NotFound();
            }
            var rotationImage = await _context.RotationImage
                .SingleOrDefaultAsync(m => m.RotationImageCode == id);
            if (rotationImage == null)
            {
                return NotFound();
            }
            return View(rotationImage);
        }
        // GET: 添加

        public IActionResult RotationImagesCreate()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
        //添加动作
        public async Task<IActionResult> RotationImagesCreates(string RotationImagesUrl, string RotationImagesJumpUrl)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (RotationImagesUrl != null)
            {
                if (_context.RotationImage.Where(a => a.RotationImageUrl == RotationImagesUrl).Count()== 0)
                {
                    if (_context.RotationImage.Where(a => a.RotationImageJumpUrl == RotationImagesJumpUrl).Count() == 0)
                    {
                        try
                        {
                            var rotationImage = new RotationImage
                            {
                                RotationImageUrl = RotationImagesUrl,
                                RotationImageJumpUrl = RotationImagesJumpUrl,
                            };
                            _context.Add(rotationImage);
                            await _context.SaveChangesAsync();
                            return Content("200");
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            return Content("404");
                        }
                    }
                    else
                    {
                        return Content("201");
                    }
                }
                else
                {
                    return Content("201");
                } 
            }
            return Content("500");
        }
        //修改动作
        public IActionResult CheckRotationImages(int? code, string RotationImagesUrl, string RotationImagesJumpUrl)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (code == null || RotationImagesUrl == null || RotationImagesJumpUrl == null)
            {
                return Content("404");
            }
            var UpdateRotationImages = _context.RotationImage.Where(a => a.RotationImageCode == code).FirstOrDefault();
            UpdateRotationImages.RotationImageUrl = RotationImagesUrl;
            UpdateRotationImages.RotationImageJumpUrl = RotationImagesJumpUrl;
            _context.SaveChanges();
            return Content("200");
        }

        // 删除
        public IActionResult DeleteRotationImages(int? code)
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
            RotationImage rotationImage = _context.RotationImage.FirstOrDefault(u => u.RotationImageCode == code);
            _context.RotationImage.Remove(rotationImage);
            _context.SaveChanges();
            return Content("200");
        }
    }
}
