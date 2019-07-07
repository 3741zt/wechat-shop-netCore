using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace ztbiyesheji.Models
{
    public class GoodsInfo//商品三级分类
    {
         [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品信息主码")]
        public int GoodsInfoCode { get; set; }
        [DisplayName("商品价格")]
        public string GoodsInfoPrice { get; set; }
        [DisplayName("商品名称")]
        public string GoodsInfoName { get; set; }
        [DisplayName("图片地址")]
        public string GoodsInfoImageUrl { get; set; }
        public ICollection<GoodsOrders> GoodsOrdersInfoCode { get; set; }
        public ICollection<GoodsDetail> GoodsInfoDetailCode { get; set; }
        public ICollection<GoodsCarts> UserCartsDetailCode { get; set; }
        public ICollection<UserGoodsCollection> UserGoodsCollectionCode { get; set; }
        public int GoodsClassificationCode { get; set; }//二级分类id作为外键

    }
}
