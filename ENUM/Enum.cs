using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models.ENUM
{
    public class Enum
    {
        //管理员性别
        public enum AdminUserSexes
        {
            [DisplayName("男")]
            男=0,
            [DisplayName("女")]
            女=1
        }
        //权限角色
        public enum AdminUserRoleType
        {
            [DisplayName("轮播图管理员")]
            轮播图管理员=0,
            [DisplayName("商品管理员")]
            商品管理员=1,
            [DisplayName("订单管理员")]
            订单管理员=2,
            [DisplayName("超级管理员")]
            超级管理员=3
        }
        //订单状态
        public enum OrderStateType
        {
            [DisplayName("未发货")]
            未发货=0,
            [DisplayName("待收货")]
            待收货=1,
            [DisplayName("已收货")]
            已收货=2,
            [DisplayName("退货中")]
            退货中=3,
            [DisplayName("已退货")]
            已退货=4
        }
    }
}
