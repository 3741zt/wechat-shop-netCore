using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;

namespace ztbiyesheji.Controllers.wxshopAPI.user
{
    public class GoodsSGController: Controller
    {
        private readonly DemoDbContext _context;
        public GoodsSGController(DemoDbContext context)
        {
            _context = context;
        }
        #region---用户收藏---

        #region---添加---
        public IActionResult AddGoodsCollection(string openid,int goodsinfocode)
        {
            if (openid == null || goodsinfocode.ToString() == null)
            {
                return NotFound();
            }
            string Openid = GetOpenid(openid);
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int Code = 0;
            foreach (var item in result)
            {
                Code = item.UserAppCode;
            }
            try
            {
                var GoodsCollection = new UserGoodsCollection
                {
                    UserAppCode=Code,
                    GoodsInfoCode= goodsinfocode
                };
                _context.UserGoodsCollection.Add(GoodsCollection);
                _context.SaveChanges();
                return Content("addsucess");
            }
            catch (DbException)
            {
                return Content("addfail");
            }
        }
        #endregion

        #region---获取---
        public IActionResult GetGoodsCollection(string openid)
        {
            if (openid == "")
            {
                return NotFound();
            }
            string Openid = GetOpenid(openid);
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int Code = 0;
            foreach (var item in result)
            {
                Code = item.UserAppCode;
            }
            var GoodsCollection = from a in _context.UserGoodsCollection
                         from b in _context.GoodsInfo
                         where a.GoodsInfoCode == b.GoodsInfoCode && a.UserAppCode == Code
                         select b;
            List<int> GoodsInfoCode = new List<int> { };
            foreach (var item in GoodsCollection)
            {
                GoodsInfoCode.Add(item.GoodsInfoCode);
            };
            var goodsInfos = _context.GoodsInfo.Where(b => GoodsInfoCode.Contains(b.GoodsInfoCode)).ToList();
            return Json(goodsInfos);
        }
        #endregion

        #endregion

        #region---用户添加购物车---

        #region---添加---
        public IActionResult AddGoodsCarts(int goodsinfocode, string openid)
        {
            if (openid == null || goodsinfocode.ToString() == null)
            {
                return NotFound();
            }
            string Openid = GetOpenid(openid);
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int Code = 0;
            foreach (var item in result)
            {
                Code = item.UserAppCode;
            }
            try
            {
                var GoodsCarts = new GoodsCarts
                {
                    UserAppCode = Code,
                    GoodsInfoCode = goodsinfocode
                };
                _context.GoodsCarts.Add(GoodsCarts);
                _context.SaveChanges();
                return Content("addsucess");
            }
            catch (DbException)
            {
                return Content("addfail");
            }
        }
        #endregion

        #region---获取---
        public IActionResult GetGoodsCarts(string openid)
        {
            if (openid == "")
            {
                return NotFound();
            }
            string Openid = GetOpenid(openid);
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int Code = 0;
            foreach (var item in result)
            {
                Code = item.UserAppCode;
            }
            var GoodsGouwuche = from a in _context.GoodsCarts
                         from b in _context.GoodsInfo
                         where a.GoodsInfoCode == b.GoodsInfoCode && a.UserAppCode == Code
                                select b;
            List<int> GoodsInfoCode = new List<int> { };
            foreach (var item in GoodsGouwuche)
            {
                GoodsInfoCode.Add(item.GoodsInfoCode);
            };
            var goodsInfos = _context.GoodsInfo.Where(b => GoodsInfoCode.Contains(b.GoodsInfoCode)).ToList();
            return Json(goodsInfos);
        }
        #endregion

        #endregion

        #region---MD5---
        public static string GetOpenid(string openid)
        {
            string Openid = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(openid));
            for (int i = 0; i < s.Length; i++)
            {
                Openid = Openid + s[i].ToString("X");
            }
            return Openid;
        }
        #endregion
    }


}
