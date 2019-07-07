using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;

namespace ztbiyesheji.Controllers
{
    public class UserController : Controller
    {
        public string password = "ae125efkk4454eeff444ferfkny6oxi8";
        private readonly DemoDbContext _context;
        public UserController(DemoDbContext context)
        {
            _context = context;
        }

        public IActionResult Adduser(string openid, string nickname)
        {
            #region---判断Openid与MD5加密---
            if (openid == null)
            {
                return Content("OpenidIsNull");
            }
            string Openid = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(openid));
            for (int i = 0; i < s.Length; i++)
            {
                Openid = Openid + s[i].ToString("X");
            }
            var name = Encrypt(nickname,password);
            #endregion

            #region---生成数据与唯一id---
            if (_context.UserApp.Where(a => a.UserAppOpenid == Openid).Count() == 0)
            {
                //<summary>
                ///Openid数据库中没有数据添加数据
                //<summary>
                try
                {
                    var B = new UserApp
                    {
                        UserAppOpenid = Openid,
                        WxName = Encrypt(nickname,password)
                    };
                    _context.UserApp.Add(B);
                    _context.SaveChanges();
                    return Content("addsucess");
                }
                catch (Exception)
                {
                    return Content("addfail");
                }
            }
            else if (!(_context.UserApp.Where(a => a.UserAppOpenid == Openid).Count() == 0 && _context.UserApp.Where(a => a.WxName == nickname).Count() == 0))
            {
                var updatename = _context.UserApp.First(c => c.UserAppOpenid == Openid);
                updatename.WxName = Encrypt(nickname, password);
                _context.SaveChanges();
            }
            return Content("exist");
        }
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

      
    }
    #endregion
}