using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;
using static ztbiyesheji.Models.ENUM.Enum;

namespace ztbiyesheji.Controllers.wxshopAPI
{
    public class OrderController : Controller
    {
        public string password = "ae125efkk4454eeff444ferfkny6oxi8";
        private readonly DemoDbContext _context;
        public OrderController(DemoDbContext context)
        {
            _context = context;
        }
        #region---订单操作---
        public IActionResult AddOrder(int goodsinfocode, string openid, int useraddresscode)
        {
            string time = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") +
          DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss");
            if (openid == null || goodsinfocode.ToString() == null)
            {
                return NotFound();
            }
            #region---MD5加密Openid
            string Openid = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(openid));
            for (int i = 0; i < s.Length; i++)
            {
                Openid = Openid + s[i].ToString("X");
            }
            #endregion

            #region---通过Openid获取用户表数据与收获地址表详细收货地址---
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int UserOrdersCode = 0;
            string WxName = "";
            foreach (var item in result)
            {
                UserOrdersCode = item.UserAppCode;
                WxName = Decrypt(item.WxName, password);
            }

            var address = from b in _context.UserAddresses
                          where b.UserAddressesCode == useraddresscode
                          select b;
            string detailaddress = "";
            string useraddressname = "";
            string useraddressphonenum = "";
            foreach (var item in address)
            {
                detailaddress = Decrypt(item.UserDetailAddresses, password);
                useraddressname = Decrypt(item.UserAddressesName, password);
                useraddressphonenum = Decrypt(item.UserAddressesPhoneNum, password);
            }
            #endregion

            #region---订单编码年月日---
            string merchant = goodsinfocode.ToString();
            merchant = merchant.PadLeft(5, '0');     // 共5位，之前用0补齐
            string num = GetRandomString(5);//自动生成一个5位随机数
            string ordernum = time + merchant + num + WxName;
            #endregion

            #region---订单表生成数据---
            try
            {
                var a = new GoodsOrders
                {
                    UserOrdersDateTime = DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss：ffff"),
                    GoodsInfoCode = goodsinfocode,
                    UserAppCode = UserOrdersCode,
                    GoodsOrdersCode = ordernum,
                    UserAddressDetail = Encrypt(detailaddress, password),
                    UserAddressesName = Encrypt(useraddressname, password),
                    UserAddressesPhoneNum = Encrypt(useraddressphonenum, password)
                };
                _context.GoodsOrders.Add(a);
                _context.SaveChanges();
                return Content("addsucess");
            }
            catch (DbException)
            {
                return Content("addfail");
            }
            #endregion
        }

        #region---随机数生成----
        public static string GetRandomString(int iLength)
        {
            string buffer = "0123456789";// 随机字符中也可以为汉字（任何）  
            StringBuilder sb = new StringBuilder();
            Random r = new Random();
            int range = buffer.Length;
            for (int i = 0; i < iLength; i++)
            {
                sb.Append(buffer.Substring(r.Next(range), 1));
            }
            return sb.ToString();
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
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #endregion

        #region---获取订单数据---
        public IActionResult Getodrer(string openid, OrderStateType state)
        {
            #region---MD5加密Openid
            string Openid = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(openid));
            for (int i = 0; i < s.Length; i++)
            {
                Openid = Openid + s[i].ToString("X");
            }
            #endregion

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
                var odrerdetail = from a in _context.GoodsOrders
                                  where a.UserAppCode == Code && a.UserOrdersState  == state
                                  select a;
                List<int> GoodsInfoCode = new List<int> { };
                foreach (var item in odrerdetail)
                {
                    GoodsInfoCode.Add(item.GoodsInfoCode); 
                };
                var odrergoodsinfo = _context.GoodsInfo.Where(b => GoodsInfoCode.Contains(b.GoodsInfoCode)).ToList();  
                return Json(odrergoodsinfo);
            }
            catch (Exception)
            {
                return Content("获取失败");
            }
        }
        #endregion
    }
}