using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;

namespace ztbiyesheji.Controllers
{
    public class GoodsController : Controller
    {
        private readonly DemoDbContext _context;

        public GoodsController(DemoDbContext context)
        {
            _context = context;
        }
        #region--商品信息--
        //商品信息列表
        public async Task<IActionResult> GoodsInfoIndex(string search, string sortOrder, string currentFilter, int? pageNumber,int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            
            var Total = _context.GoodsInfo.ToList().Count();
            ViewBag.Total = Total;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = search;
            var a = ViewData["CurrentFilter"];
            if (search == null && a == null && state==null)
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
            var GoodsInfos = from s in _context.GoodsInfo
                             select s;
            if (!String.IsNullOrEmpty(search))
            {
                GoodsInfos = GoodsInfos.Where(s => s.GoodsInfoName.Contains(search)
                                       || s.GoodsInfoPrice.Contains(search) || s.GoodsClassificationCode.ToString() == search);
            }
            List<int> GoodsClassCode = new List<int> { };
            foreach (var item in GoodsInfos)
            {
                GoodsClassCode.Add(item.GoodsClassificationCode);
            };
            var goodsClassifications = _context.GoodsClassification.Where(b => GoodsClassCode.Contains(b.GoodsClassificationCode)).ToList();
            ViewBag.goodsClassifications = goodsClassifications;
            switch (sortOrder)
            {
                case "GoodsInfoNumber":
                    GoodsInfos = GoodsInfos.OrderByDescending(s => s.GoodsInfoCode);
                    break;
                case "GoodsInfoName":
                    GoodsInfos = GoodsInfos.OrderBy(s => s.GoodsInfoName);
                    break;
            }
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = GoodsInfos.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            if (pageNumber > Totalpage )
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<GoodsInfo>.CreateAsync(GoodsInfos.AsNoTracking(), pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0 /*|| (SearchCount < pageSize && search != null)*/)
            {
                return View(await PaginatedList<GoodsInfo>.CreateAsync(GoodsInfos.AsNoTracking(), 1, pageSize));
            }
            return View(await PaginatedList<GoodsInfo>.CreateAsync(GoodsInfos.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //商品信息详情
        public async Task<IActionResult> GoodsInfosDetails(int? Id)
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

            var GoodsInfo = await _context.GoodsInfo
                .SingleOrDefaultAsync(m => m.GoodsInfoCode == Id);
            var Name= await _context.GoodsClassification
                .SingleOrDefaultAsync(m => m.GoodsClassificationCode == GoodsInfo.GoodsClassificationCode);
            ViewBag.Name = Name.GoodsClassificationName;
            if (GoodsInfo == null)
            {
                return NotFound();
            }

            return View(GoodsInfo);
        }

        //修改
        public IActionResult CheckGoodsInfos(int? ClassCode,string GoodsInfoName,string GoodsInfoPrice,string GoodsInfoImageUrl,int? Code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (ClassCode == null || GoodsInfoName == null || GoodsInfoPrice == null || GoodsInfoImageUrl == null)
            {
                return Content("404");
            }
            if (_context.GoodsInfo.Where(a => a.GoodsInfoName == GoodsInfoName).Count() == 0 || _context.GoodsInfo.Where(a => a.GoodsInfoImageUrl == GoodsInfoImageUrl).Count() == 0)
            {
                try
                {
                    var UpdateGoodsInfos = _context.GoodsInfo.Where(a => a.GoodsInfoCode == Code).FirstOrDefault();
                    UpdateGoodsInfos.GoodsInfoName = GoodsInfoName;
                    UpdateGoodsInfos.GoodsInfoPrice = GoodsInfoPrice;
                    UpdateGoodsInfos.GoodsInfoImageUrl = GoodsInfoImageUrl;
                    UpdateGoodsInfos.GoodsClassificationCode = Convert.ToInt32(ClassCode);
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
                return Content("201");
            }
        }

        // 添加
        public async Task<IActionResult> GoodsInfosCreate()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(await _context.GoodsClassification.ToListAsync());
        }

        //添加动作
        public async Task<IActionResult> GoodsInfosCreates(int? ClassCode, string GoodsInfoName, string GoodsInfoPrice, string GoodsInfoImageUrl)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (ClassCode == null || GoodsInfoName == null || GoodsInfoPrice == null || GoodsInfoImageUrl == null)
            {
                return Content("404");
            }
            if (_context.GoodsInfo.Where(a => a.GoodsInfoName == GoodsInfoName).Count() == 0 || _context.GoodsInfo.Where(a => a.GoodsInfoImageUrl == GoodsInfoImageUrl).Count() == 0)
            {
                try
                {

                    var GoodsInfo = new GoodsInfo
                    {
                        GoodsInfoName = GoodsInfoName,
                        GoodsInfoPrice = GoodsInfoPrice,
                        GoodsInfoImageUrl = GoodsInfoImageUrl,
                       GoodsClassificationCode=Convert.ToInt32(ClassCode),
                    };
                    _context.Add(GoodsInfo);
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
        public IActionResult DeleteGoodsInfos(int? code)
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
            var GoodsDetail=  _context.GoodsDetail
                .FirstOrDefault(m => m.GoodsInfoCode == code);
            var GoodsOrders= _context.GoodsOrders
                .FirstOrDefault(m => m.GoodsInfoCode == code);
            if (GoodsDetail != null || GoodsOrders !=null)
            {
                return Content("404");
            }
            GoodsInfo GoodsInfo = _context.GoodsInfo.FirstOrDefault(u => u.GoodsInfoCode == code);
            _context.GoodsInfo.Remove(GoodsInfo);
            _context.SaveChanges();
            return Content("200");
        }
        #endregion

        #region--商品品牌分类--
        //商品品牌列表
        public async Task<IActionResult> GoodsClassificationIndex(string search, string sortOrder, string currentFilter, int? pageNumber, int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var Total = _context.GoodsClassification.ToList().Count();
            var Totals = _context.GoodsClassification.ToList();
            List<int> CommonCode = new List<int> { };
            foreach (var item in Totals)
            {
                CommonCode.Add(item.GoodsClassificationCode);
            };
            var GoodsCommonTains = _context.GoodsCommodities.Where(b => CommonCode.Contains(b.GoodsCommoditiesCode)).ToList();
            ViewBag.GoodsCommonTains = GoodsCommonTains;
            ViewBag.Total = Total;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = search;
            ViewBag.SearchName = GoodsCommonTains.FirstOrDefault(m => m.GoodsCommoditiesCode == Convert.ToInt32(search));
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
            var GoodsClassification = from s in _context.GoodsClassification
                                    select s;
            if (!String.IsNullOrEmpty(search))
            {
                GoodsClassification = GoodsClassification.Where(s => s.GoodsCommoditiesCode == Convert.ToInt32(search));
            }
            List<int> GoodsCommoditiees = new List<int> { };
            foreach (var item in GoodsClassification)
            {
                GoodsCommoditiees.Add(item.GoodsClassificationCode);
            };
            var GoodsCommodities = _context.GoodsCommodities.Where(b => GoodsCommoditiees.Contains(b.GoodsCommoditiesCode)).ToList();
            ViewBag.GoodsCommodities = GoodsCommodities;
            //switch (sortOrder)
            //{
            //    case "GoodsInfoNumber":
            //        GoodsDetailImages = GoodsDetailImages.OrderByDescending(s => s.GoodsInfoCode);
            //        break;
            //    case "GoodsInfoName":
            //        GoodsDetailImages = GoodsDetailImages.OrderBy(s => s.GoodsInfoName);
            //        break;
            //}
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = GoodsClassification.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            if (pageNumber > Totalpage)
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<GoodsClassification>.CreateAsync(GoodsClassification.AsNoTracking(), pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0 /*|| (SearchCount < pageSize && search != null)*/)
            {
                return View(await PaginatedList<GoodsClassification>.CreateAsync(GoodsClassification.AsNoTracking(), 1, pageSize));
            }
            return View(await PaginatedList<GoodsClassification>.CreateAsync(GoodsClassification.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //商品品牌详情
        public async Task<IActionResult> GoodsClassificationDetails(int? Id)
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

            var GoodsCommoditie = await _context.GoodsClassification
                .SingleOrDefaultAsync(m => m.GoodsClassificationCode == Id);
            var Name = await _context.GoodsCommodities
                .SingleOrDefaultAsync(m => m.GoodsCommoditiesCode == GoodsCommoditie.GoodsCommoditiesCode);
            ViewBag.Name = Name.GoodsCommoditiesName;
            if (GoodsCommoditie == null)
            {
                return NotFound();
            }

            return View(GoodsCommoditie);
        }

        //修改商品品牌
        public IActionResult CheckGoodsClassification(int? GoodsCommoditiesCode, string GoodsClassificationName, string GoodsClassificationImageUrl, int? Code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (GoodsCommoditiesCode == null || GoodsClassificationName == null  || Code == null)
            {
                return Content("404");
            }
            if (_context.GoodsClassification.Where(a => a.GoodsClassificationName == GoodsClassificationName).Count() == 0)
            {
                try
                {
                    var UpdateGoodsClassifications = _context.GoodsClassification.Where(a => a.GoodsClassificationCode == Code).FirstOrDefault();
                    UpdateGoodsClassifications.GoodsClassificationName = GoodsClassificationName;
                    UpdateGoodsClassifications.GoodsClassificationImageUrl = GoodsClassificationImageUrl;
                    UpdateGoodsClassifications.GoodsCommoditiesCode = Convert.ToInt32(GoodsCommoditiesCode);
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
                return Content("201");
            }
        }

        // 添加商品品牌
        public IActionResult GoodsClassificationCreate()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        //添加动作
        public async Task<IActionResult> GoodsClassificationCreates(int? GoodsCommoditiesCode, string GoodsClassificationName,  string GoodsClassificationImageUrl)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (GoodsCommoditiesCode == null || GoodsClassificationName == null)
            {
                return Content("404");
            }
            if (_context.GoodsClassification.Where(a => a.GoodsClassificationName == GoodsClassificationName).Count() == 0 )
            {
                try
                {
                    var GoodsClassification = new GoodsClassification
                    {
                        GoodsClassificationName = GoodsClassificationName,
                        GoodsClassificationImageUrl = GoodsClassificationImageUrl,
                        GoodsCommoditiesCode = Convert.ToInt32(GoodsCommoditiesCode),
                    };
                    _context.Add(GoodsClassification);
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
        public IActionResult DeleteGoodsClassification(int? code)
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
            var GoodsInfo = _context.GoodsInfo
                .FirstOrDefault(m => m.GoodsClassificationCode == code);
            
            if (GoodsInfo != null)
            {
                return Content("404");
            }
            GoodsClassification goodsClassification = _context.GoodsClassification.FirstOrDefault(u => u.GoodsClassificationCode == code);
            _context.GoodsClassification.Remove(goodsClassification);
            _context.SaveChanges();
            return Content("200");
        }
        #endregion

        #region--商品详细信息图片--
        //商品详细信息图片列表
        public async Task<IActionResult> GoodsDetailImageIndex(string search, string sortOrder, string currentFilter, int? pageNumber, int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var Total = _context.GoodsDetailImage.ToList().Count();
            var Totals = _context.GoodsDetailImage.ToList();
            List<int> GoodsDetailsTotal = new List<int> { };
            foreach (var item in Totals)
            {
                GoodsDetailsTotal.Add(item.GoodsDetailCode);
            };
            var GoodsDetailsTotales = _context.GoodsDetail.Where(b => GoodsDetailsTotal.Contains(b.GoodsDetailCode)).ToList();
            ViewBag.GoodsDetailsTotales = GoodsDetailsTotales;
            ViewBag.Total = Total;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = search;
            ViewBag.SearchName = GoodsDetailsTotales.FirstOrDefault(m => m.GoodsDetailCode == Convert.ToInt32(search));
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
            var GoodsDetailImages = from s in _context.GoodsDetailImage
                             select s;
            if (!String.IsNullOrEmpty(search))
            {
                GoodsDetailImages = GoodsDetailImages.Where(s => s.GoodsDetailCode==Convert.ToInt32(search));
            }
            List<int> GoodsDetails = new List<int> { };
            foreach (var item in GoodsDetailImages)
            {
                GoodsDetails.Add(item.GoodsDetailCode);
            };
            var GoodsDetailses = _context.GoodsDetail.Where(b => GoodsDetails.Contains(b.GoodsDetailCode)).ToList();
            ViewBag.GoodsDetailses = GoodsDetailses;
            //switch (sortOrder)
            //{
            //    case "GoodsInfoNumber":
            //        GoodsDetailImages = GoodsDetailImages.OrderByDescending(s => s.GoodsInfoCode);
            //        break;
            //    case "GoodsInfoName":
            //        GoodsDetailImages = GoodsDetailImages.OrderBy(s => s.GoodsInfoName);
            //        break;
            //}
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = GoodsDetailImages.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            if (pageNumber > Totalpage)
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<GoodsDetailImage>.CreateAsync(GoodsDetailImages.AsNoTracking(), pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0 /*|| (SearchCount < pageSize && search != null)*/)
            {
                return View(await PaginatedList<GoodsDetailImage>.CreateAsync(GoodsDetailImages.AsNoTracking(), 1, pageSize));
            }
            return View(await PaginatedList<GoodsDetailImage>.CreateAsync(GoodsDetailImages.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //商品详细信息图片详情
        public async Task<IActionResult> GoodsDetailImageDetails(int? Id)
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
            var GoodsDetailImage = await _context.GoodsDetailImage
                .SingleOrDefaultAsync(m => m.GoodsDetailImageCode == Id);
            var Name = await _context.GoodsDetail
                .SingleOrDefaultAsync(m => m.GoodsDetailCode == GoodsDetailImage.GoodsDetailCode);
            ViewBag.Name = Name.GoodsDetailName;
            if (GoodsDetailImage == null)
            {
                return NotFound();
            }

            return View(GoodsDetailImage);
        }

        //商品详细信息图片修改
        public IActionResult CheckGoodsDetailImage(int? GoodsDetailCode, string GoodsDetailImageUrl, string GoodsDetailXImageUrl,  int? Code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (GoodsDetailCode == null || GoodsDetailImageUrl == null || GoodsDetailXImageUrl == null || Code == null)
            {
                return Content("404");
            }
            if (_context.GoodsDetailImage.Where(a => a.GoodsDetailImageUrl == GoodsDetailImageUrl).Count() == 0 || _context.GoodsDetailImage.Where(a => a.GoodsDetailXImageUrl == GoodsDetailXImageUrl).Count() == 0)
            {
                try
                {
                    var UpdateGoodsDetailImage = _context.GoodsDetailImage.Where(a => a.GoodsDetailImageCode == Code).FirstOrDefault();
                    UpdateGoodsDetailImage.GoodsDetailImageUrl = GoodsDetailImageUrl;
                    UpdateGoodsDetailImage.GoodsDetailXImageUrl = GoodsDetailXImageUrl;
                    UpdateGoodsDetailImage.GoodsDetailCode = Convert.ToInt32(GoodsDetailCode);
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
                return Content("201");
            }
        }

        // 商品详细信息图片添加
        public async Task<IActionResult> GoodsDetailImageCreate()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(await _context.GoodsDetail.ToListAsync());
        }

        //商品详细信息图片添加动作
        public async Task<IActionResult> GoodsDetailImageCreates(int? GoodsDetailCode, string GoodsDetailImageUrl, string GoodsDetailXImageUrl)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (GoodsDetailCode == null || GoodsDetailImageUrl == null || GoodsDetailXImageUrl == null )
            {
                return Content("404");
            }
            if (_context.GoodsDetailImage.Where(a => a.GoodsDetailImageUrl == GoodsDetailImageUrl).Count() == 0 || _context.GoodsDetailImage.Where(a => a.GoodsDetailXImageUrl == GoodsDetailXImageUrl).Count() == 0)
            {
                try
                {

                    var GoodsDetailImage = new GoodsDetailImage
                    {
                        GoodsDetailImageUrl = GoodsDetailImageUrl,
                        GoodsDetailXImageUrl = GoodsDetailXImageUrl,
                        GoodsDetailCode = Convert.ToInt32(GoodsDetailCode),
                    };
                    _context.Add(GoodsDetailImage);
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


        // 商品详细信息图片删除
        public IActionResult DeleteGoodsDetailImage(int? code)
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
            GoodsDetailImage GoodsDetailImage = _context.GoodsDetailImage.FirstOrDefault(u => u.GoodsDetailImageCode == code);
            _context.GoodsDetailImage.Remove(GoodsDetailImage);
            _context.SaveChanges();
            return Content("200");
        }
        #endregion

        #region--商品详细信息--
        //商品信息列表
        public async Task<IActionResult> GoodsDetailIndex(string search, string sortOrder, string currentFilter, int? pageNumber, int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var Total = _context.GoodsDetail.ToList().Count();
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
            var GoodsDetail = from s in _context.GoodsDetail
                              select s;
            if (!String.IsNullOrEmpty(search))
            {
                GoodsDetail = GoodsDetail.Where(s => s.GoodsDetailName.Contains(search)
                                       || s.GoodsDetailPrice.Contains(search));
            }
            List<int> GoodsInfoCodes = new List<int> { };
            foreach (var item in GoodsDetail)
            {
                GoodsInfoCodes.Add(item.GoodsInfoCode);
            };
            var goodsInfoes = _context.GoodsInfo.Where(b => GoodsInfoCodes.Contains(b.GoodsInfoCode)).ToList();
            ViewBag.goodsInfoes = goodsInfoes;
            //switch (sortOrder)
            //{
            //    case "GoodsInfoNumber":
            //        GoodsInfos = GoodsInfos.OrderByDescending(s => s.GoodsInfoCode);
            //        break;
            //    case "GoodsInfoName":
            //        GoodsInfos = GoodsInfos.OrderBy(s => s.GoodsInfoName);
            //        break;
            //}
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = GoodsDetail.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            if (pageNumber > Totalpage)
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<GoodsDetail>.CreateAsync(GoodsDetail.AsNoTracking(), pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0 /*|| (SearchCount < pageSize && search != null)*/)
            {
                return View(await PaginatedList<GoodsDetail>.CreateAsync(GoodsDetail.AsNoTracking(), 1, pageSize));
            }
            return View(await PaginatedList<GoodsDetail>.CreateAsync(GoodsDetail.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //商品信息详情
        public async Task<IActionResult> GoodsDetailDetails(int? Id)
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

            var GoodsDetails = await _context.GoodsDetail
                .SingleOrDefaultAsync(m => m.GoodsDetailCode == Id);
            var Name = await _context.GoodsInfo
                .SingleOrDefaultAsync(m => m.GoodsInfoCode == GoodsDetails.GoodsInfoCode);
            ViewBag.Name = Name.GoodsInfoName;
            if (GoodsDetails == null)
            {
                return NotFound();
            }

            return View(GoodsDetails);
        }

        //修改
        public IActionResult CheckGoodsDetails(int? GoodsInfoCode,string GoodsDetailName,string GoodsDetailPrice, string GoodsDetailSize,string GoodsDetailColor,int? code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (GoodsInfoCode == null || GoodsDetailName == null || GoodsDetailPrice == null || code == null)
            {
                return Content("404");
            }
            if (_context.GoodsDetail.Where(a => a.GoodsDetailName == GoodsDetailName).Count() == 0 )
            {
                try
                {
                    var UpdateGoodsDetails = _context.GoodsDetail.Where(a => a.GoodsDetailCode == code).FirstOrDefault();
                    UpdateGoodsDetails.GoodsDetailName = GoodsDetailName;
                    UpdateGoodsDetails.GoodsDetailPrice = GoodsDetailPrice;
                    UpdateGoodsDetails.GoodsDetailSize = GoodsDetailSize;
                    UpdateGoodsDetails.GoodsDetailColor = GoodsDetailColor;
                    UpdateGoodsDetails.GoodsInfoCode = Convert.ToInt32(GoodsInfoCode);
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
                return Content("201");
            }
        }

        // 添加
        public async Task<IActionResult> GoodsDetailCreate()
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View(await _context.GoodsInfo.ToListAsync());
        }

        //添加动作
        public async Task<IActionResult> GoodsDetailsCreates(int? GoodsInfoCode, string GoodsDetailName, string GoodsDetailPrice, string GoodsDetailSize, string GoodsDetailColor)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (GoodsInfoCode == null || GoodsDetailName == null || GoodsDetailPrice == null)
            {
                return Content("404");
            }
            if (_context.GoodsDetail.Where(a => a.GoodsDetailName == GoodsDetailName).Count() == 0|| _context.GoodsDetail.Where(a => a.GoodsInfoCode == GoodsInfoCode).Count() == 0)
            {
                try
                {

                    var GoodsDetail = new GoodsDetail
                    {
                        GoodsDetailName = GoodsDetailName,
                        GoodsDetailPrice = GoodsDetailPrice,
                        GoodsDetailSize = GoodsDetailSize,
                        GoodsDetailColor= GoodsDetailColor,
                        GoodsInfoCode = Convert.ToInt32(GoodsInfoCode),
                    };
                    _context.Add(GoodsDetail);
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
        public IActionResult DeleteGoodsDetails(int? code)
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
            var GoodDetailImage = _context.GoodsDetailImage
                .FirstOrDefault(m => m.GoodsDetailCode == code);
            if (GoodDetailImage != null)
            {
                return Content("404");
            }
            GoodsDetail goodsDetails = _context.GoodsDetail.FirstOrDefault(u => u.GoodsDetailCode == code);
            _context.GoodsDetail.Remove(goodsDetails);
            _context.SaveChanges();
            return Content("200");
        }
        #endregion
    }
}