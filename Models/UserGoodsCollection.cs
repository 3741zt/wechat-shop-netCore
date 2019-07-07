using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class UserGoodsCollection
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("用户收藏主码")]
        public int UserGoodsCollectionCode { get; set; }
        [DisplayName("收藏人")]
        public int UserAppCode { get; set; }
        [DisplayName("收藏商品")]
        public int GoodsInfoCode { get; set; }
    }
}
