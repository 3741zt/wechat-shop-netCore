using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class UserApp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("用户主码")]
        public int UserAppCode { get; set; }

        [DisplayName(" 用户唯一标识（微信小程序）")]
        public string UserAppOpenid { get; set; }

        [DisplayName(" 用户微信姓名")]
        public string WxName { get; set; }


        #region---外键关系---
        //<summary>
        ///用户购物车关系
        //<summary>
        public ICollection<GoodsCarts> GoodsCarts { get; set; }

        /// <summary>
        /// 用户收获地址关系
        /// </summary>
        public ICollection<UserAddresses> UserAppAddressesCode { get; set; }

        /// <summary>
        /// 用户订单关系
        /// </summary>
        public ICollection<GoodsOrders> UserOrdersDetailCode { get; set; }

        /// <summary>
        /// 用户收藏关系
        /// </summary>
        public ICollection<UserGoodsCollection> GoodsCollectionCode { get; set; }
        #endregion
    }
}
