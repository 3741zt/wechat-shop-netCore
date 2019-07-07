using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static ztbiyesheji.Models.ENUM.Enum;

namespace ztbiyesheji.Models
{
    public class GoodsOrders//订单表
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品订单主码")]
        public string GoodsOrdersCode { get; set; }
        [DisplayName("订单状态")]
        public OrderStateType UserOrdersState { get; set; }
        [DisplayName("商品订单生成时间")]
        public string UserOrdersDateTime { get; set; }
        [DisplayName("订单所属商品")]
        public int GoodsInfoCode { get; set; }
        [DisplayName("订单所属人")]
        public int UserAppCode { get; set; }
        [DisplayName("用户收获地址")]
        public string UserAddressDetail { get; set; }
        [DisplayName("用户收获姓名")]
        public string UserAddressesName { get; set; }
        [DisplayName("用户收获电话")]
        public string UserAddressesPhoneNum { get; set; }
    }
}
