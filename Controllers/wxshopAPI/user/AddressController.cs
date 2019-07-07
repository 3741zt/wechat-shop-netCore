using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;

namespace ztbiyesheji.Controllers
{
    public class AddressController : Controller
    {
        public string password = "ae125efkk4454eeff444ferfkny6oxi8";
        private readonly DemoDbContext _context;
        public AddressController(DemoDbContext context)
        {
            _context = context;
        }
        #region---添加/修改收获地址---
        public IActionResult Addaddress(string Name, string PhoneNum, string Detailedaddress, string openid)
        {

            #region---MD5加密Openid与找到主码
            string Openid = GetMD5(openid);
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int Code = 0;
            foreach (var item in result)
            {
                Code = item.UserAppCode;
            }
            #endregion

            #region---添加数据/修改数据---
            int NameCount = _context.UserAddresses.Where(a => a.UserAddressesName == Encrypt(Name, password)).Count();
            int PhoneNumCount = _context.UserAddresses.Where(a => a.UserAddressesPhoneNum == Encrypt(PhoneNum, password)).Count();
            int DetailedaddressCount = _context.UserAddresses.Where(a => a.UserDetailAddresses == Encrypt(Detailedaddress, password)).Count();
            if (
                (NameCount == 0 && PhoneNumCount == 0 && DetailedaddressCount == 0)
                ||
                ((NameCount == 0 && PhoneNumCount == 0 && !(DetailedaddressCount == 0)) || (NameCount == 0 && !(PhoneNumCount == 0) && DetailedaddressCount == 0) || (!(NameCount == 0) && PhoneNumCount == 0 && DetailedaddressCount == 0))
                ||
                ((!(NameCount == 0 && PhoneNumCount == 0) && DetailedaddressCount == 0) || (!(NameCount == 0 && DetailedaddressCount == 0) && PhoneNumCount == 0) || (!(PhoneNumCount == 0 && DetailedaddressCount == 0) && NameCount == 0))
                )
            {
                try
                {
                    var a = new UserAddresses
                    {
                        UserAddressesName = Encrypt(Name, password),
                        UserAddressesPhoneNum = Encrypt(PhoneNum, password),
                        UserDetailAddresses = Encrypt(Detailedaddress, password),
                        UserAppCode = Code
                    };
                    _context.UserAddresses.Add(a);
                    _context.SaveChanges();
                    return Content("200");
                }
                catch (Exception)
                {

                    return Content("500");
                }
            }
            else
            {
                return Content("400");
            }
            #endregion
        }
        #endregion

        #region---获取用户收货数据---
        public IActionResult Getaddress(string openid)
        {
            string Openid = GetMD5(openid);
            var result = from a in _context.UserApp
                         where a.UserAppOpenid == Openid
                         select a;
            int UserAppCode = 0;
            foreach (var item in result)
            {
                UserAppCode = item.UserAppCode;
            }
            var address = from a in _context.UserAddresses
                          where a.UserAppCode == UserAppCode
                          select a;
            if (address != null)
            {
                foreach (var item in address)
                {
                    item.UserAddressesName = Decrypt(item.UserAddressesName, password);

                    item.UserAddressesPhoneNum = Decrypt(item.UserAddressesPhoneNum, password);

                    item.UserDetailAddresses = Decrypt(item.UserDetailAddresses, password);
                }
            }
            return Json(address);
        }
        #endregion

        #region---获取单个数据---
        public IActionResult GetOneAddress(int code)
        {
            
            var address = from a in _context.UserAddresses
                          where a.UserAddressesCode == code
                          select a;
            if (address != null)
            {
                foreach (var item in address)
                {
                    item.UserAddressesName = Decrypt(item.UserAddressesName, password);

                    item.UserAddressesPhoneNum = Decrypt(item.UserAddressesPhoneNum, password);

                    item.UserDetailAddresses = Decrypt(item.UserDetailAddresses, password);
                }
            }
            return Json(address);
        }
        #endregion

        #region---用户修改收货地址---
        public IActionResult Changeaddress( string Name, string PhoneNum, string Detailedaddress, int code)
        {
            try
            {
                var UpdateAddress = _context.UserAddresses.Where(u => u.UserAddressesCode == code).FirstOrDefault();
                UpdateAddress.UserAddressesName = Encrypt(Name, password);
                UpdateAddress.UserAddressesPhoneNum = Encrypt(PhoneNum, password);
                UpdateAddress.UserDetailAddresses = Encrypt(Detailedaddress, password);
                _context.SaveChanges();
                return Content("updateaddresssuccess!");
            }
            catch (Exception)
            {
                return Content("updateaddressfail");
            }
        }
        #endregion

        #region---用户删除收货地址---
        public IActionResult RemoveUserAddress(int code)
        {
            try
            {
                UserAddresses RemoveUserAddress = _context.UserAddresses.FirstOrDefault(u => u.UserAddressesCode == code);
                if (RemoveUserAddress != null)
                {
                    _context.UserAddresses.Remove(RemoveUserAddress);
                    _context.SaveChanges();
                    return Content("removesuccess");

                }
                else
                {
                    return Content("没有这项数据，请联系客服!");
                }
                
            }
            catch (Exception)
            {
                return Content("removefail");
            }
        }
        #endregion

        #region---MD5封装---
        public static string GetMD5(string openid)
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