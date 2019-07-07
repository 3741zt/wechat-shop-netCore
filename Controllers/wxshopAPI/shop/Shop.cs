using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;
using System.Linq;
namespace ztbiyesheji.Controllers.wxshopAPI
{
    public class Shop: Controller
    {
        private readonly DemoDbContext _context;
        public Shop(DemoDbContext context)
        {
            _context = context;
        }
        #region---获取商品属性
        public async Task<IActionResult> GetOne()
        {
            List<GoodsCommodities> ClassList = await _context.GoodsCommodities.ToListAsync();
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            return Json(ClassList, settings);
        }
        #endregion

        #region---获取商品品牌分类---
        public async Task<IActionResult> GetTwo()
        {
            List<GoodsClassification> ClassList = await _context.GoodsClassification.ToListAsync();
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            return Json(ClassList, settings);
        }
        #endregion

        #region---获取商品属性---
        public IActionResult GetGoodsInfo(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var result = from a in _context.GoodsInfo
                         from b in _context.GoodsClassification
                         where a.GoodsClassificationCode == b.GoodsClassificationCode && b.GoodsCommoditiesCode == id
                         select a;
            return Json(result);
        }
        #endregion

        #region---获取商品详细属性---
        public IActionResult GetGoodsDetail(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var result = from a in _context.GoodsDetail
                         where a.GoodsInfoCode == id
                         select a;
            return Json(result);
        }
        #endregion

        #region---获取商品详细属性图片
        public IActionResult GetGoodsDetailImage(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var result = from a in _context.GoodsDetailImage
                         from b in _context.GoodsDetail
                         where a.GoodsDetailCode == b.GoodsDetailCode && b.GoodsInfoCode == id
                         select a;
            return Json(result);
        }
        #endregion

        #region---获取品牌数据---
        public IActionResult GetGoodsInfototal(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var result = from a in _context.GoodsInfo
                         from b in _context.GoodsClassification
                         where a.GoodsClassificationCode == b.GoodsClassificationCode && b.GoodsClassificationCode == id
                         select a;
            return Json(result);
        }
        #endregion
    }
}
