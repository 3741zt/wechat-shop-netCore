using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class GoodsDetail//商品详情页，分类四
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品详细信息主码")]
        public int GoodsDetailCode { get; set; }
        [DisplayName("商品名称")]
        public string GoodsDetailName { get; set; }
        [DisplayName("商品价格")]
        public string GoodsDetailPrice { get; set; }
        [DisplayName("商品尺码")]
        public string GoodsDetailSize { get; set; }
        [DisplayName("商品颜色")]
        public string GoodsDetailColor { get; set; }
        [DisplayName(" 外码—关联GoodsInfo表")]
        public int GoodsInfoCode { get; set; }
        public ICollection<GoodsDetailImage> GoodsDetailICode { get; set; }
    }
}
