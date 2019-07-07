using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class GoodsDetailImage//商品分类四图片存放地
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品详细信息图片主码")]
        public int GoodsDetailImageCode { get; set; }
        [DisplayName("商品详细信息轮播图")]
        public string GoodsDetailImageUrl { get; set; }
        [DisplayName("商品详细信息图")]
        public string GoodsDetailXImageUrl { get; set; }
        public int GoodsDetailCode { get; set; }
    }
}
