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
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;
using static ztbiyesheji.Models.ENUM.Enum;

namespace ztbiyesheji.Views.GoodsOrder
{
    public class GoodsOrderController : Controller
    {
        public string password = "ae125efkk4454eeff444ferfkny6oxi8";
        private readonly DemoDbContext _context;

        public GoodsOrderController(DemoDbContext context)
        {
            _context = context;
        }

        #region--订单列表--
        public async Task<IActionResult> GoodsOrdersIndex(string search, string sortOrder, string currentFilter, int? pageNumber, int? state)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            var Total = _context.GoodsOrders.ToList().Count();
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
            var GoodsOrderss = from s in _context.GoodsOrders
                               select s;
            IQueryable<GoodsOrders> NewGoodsOrders = Get(GoodsOrderss);
            if (!String.IsNullOrEmpty(search))
            {
                NewGoodsOrders = NewGoodsOrders.Where(s => s.UserAddressesPhoneNum.Contains(Encrypt(search,password))
                                       || s.UserAddressesName.Contains(Encrypt(search, password))  || s.UserOrdersState.ToString() == search || s.GoodsOrdersCode.Contains(search));
            }
            switch (sortOrder)
            {
                case "UserOrdersState":
                    NewGoodsOrders = NewGoodsOrders.OrderByDescending(s => s.UserOrdersState);
                    break;
                case "UserOrdersDateTime":
                    NewGoodsOrders = NewGoodsOrders.OrderBy(s => s.UserOrdersDateTime);
                    break;
            }
            int pageSize = 8;
            var Totalpage = (int)Math.Ceiling(Total / (double)pageSize);
            var SearchCount = NewGoodsOrders.Count();
            ViewBag.SearchCount = SearchCount;
            var SearchPage = (int)Math.Ceiling(SearchCount / (double)pageSize);
            ViewBag.SearchPage = SearchPage;
            ViewBag.Totalpage = Totalpage;
            List<int> GoodsInfoCode = new List<int> { };
            foreach (var item in NewGoodsOrders)
            {
                GoodsInfoCode.Add(item.GoodsInfoCode);
            };
            var odrergoodsinfo = _context.GoodsInfo.Where(b => GoodsInfoCode.Contains(b.GoodsInfoCode)).ToList();
            ViewBag.odrergoodsinfo = odrergoodsinfo;
            List<int> GoodsUser = new List<int> { };
            foreach (var item in NewGoodsOrders)
            {
                GoodsUser.Add(item.UserAppCode);
            };
            var user = _context.UserApp.Where(b => GoodsUser.Contains(b.UserAppCode)).ToList();
            List<UserApp> DecryptUser = Decrypts(user);
            ViewBag.user = DecryptUser;
            if (pageNumber > Totalpage)
            {
                var pageNumbers = pageNumber - 1;
                return View(await PaginatedList<GoodsOrders>.CreateAsync(NewGoodsOrders, pageNumbers.Value, pageSize));
            }
            if (pageNumber == 0)
            {
                return View(await PaginatedList<GoodsOrders>.CreateAsync(NewGoodsOrders, 1, pageSize));
            }
            return View(await PaginatedList<GoodsOrders>.CreateAsync(NewGoodsOrders, pageNumber ?? 1, pageSize));
        }

        private List<UserApp> Decrypts(List<UserApp> user)
        {
            foreach (var item in user)
            {
                item.UserAppCode = item.UserAppCode;
                item.UserAppOpenid = item.UserAppOpenid;
                item.WxName = Decrypt(item.WxName, password);
            };
            return user;
        }

        private IQueryable<GoodsOrders> Get(IQueryable<GoodsOrders> goodsOrderss)
        {
            foreach (var item in goodsOrderss)
            {
                item.GoodsOrdersCode = item.GoodsOrdersCode;
                item.UserAddressesName = Decrypt(item.UserAddressesName, password);
                item.UserAddressesPhoneNum = Decrypt(item.UserAddressesPhoneNum, password);
                item.UserOrdersState = item.UserOrdersState;
                item.UserOrdersDateTime = item.UserOrdersDateTime;
                item.UserAddressDetail = Decrypt(item.UserAddressDetail, password);
            };
            return goodsOrderss;
        }
        #endregion

        #region--订单发货--
        public IActionResult CheckGoodsOrders(OrderStateType orderState, string Code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (orderState.ToString()==null || Code==null)
            {
                return Content("401");
            }
            try
            {
                var UpdateGoodsOrders = _context.GoodsOrders.Where(a => a.GoodsOrdersCode == Code).FirstOrDefault();
                UpdateGoodsOrders.UserOrdersState = orderState;
                _context.SaveChanges();
                return Content("200");
            }
            catch (DbUpdateConcurrencyException)
            {
                return Content("500");
            }
        }
        #endregion

        #region---订单删除---
        public IActionResult DeleteGoodsOrders(string Code)
        {
            var Aid = HttpContext.Session.GetString("Aid");
            if (Aid == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Code == null)
            {
                return Content("404");
            }
            try
            {
                GoodsOrders DeleteGoodsOrder = _context.GoodsOrders.FirstOrDefault(u => u.GoodsOrdersCode == Code);
                _context.GoodsOrders.Remove(DeleteGoodsOrder);
                _context.SaveChanges();
                return Content("200");
            }
            catch (DbUpdateConcurrencyException)
            {
                return Content("500");
            }
        }
        #endregion

        #region---ASE加密---
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Encrypt(string encryptStr, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #region---ASE解密---
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Decrypt(string decryptStr, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(decryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        #endregion
    }
}
