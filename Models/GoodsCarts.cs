using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class GoodsCarts//购物车表（也作为收藏表，两张表的内容一致）
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("用户购物车主码")]
        public int GoodsCartsCode { get; set; }
        [DisplayName("购物车所属人")]
        public int UserAppCode { get; set; }
        [DisplayName("购物车商品")]
        public int GoodsInfoCode { get; set; }
    }
}
