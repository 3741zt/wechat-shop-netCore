using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class GoodsCommodities//商品一级分类
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品总分类主码")]
        public int GoodsCommoditiesCode { get; set; }
        [DisplayName(" 商品总分类名称（衣服，裤子，鞋子，其他）")]
        public string GoodsCommoditiesName { get; set; }
        public ICollection<GoodsClassification> GoodsCode { get; set; }
    }
}
