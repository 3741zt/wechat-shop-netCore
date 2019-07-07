using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class GoodsClassification//二级分类
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("商品细分类主码")]
        public int GoodsClassificationCode { get; set; }
        [DisplayName("商品细分类名称")]
        public string GoodsClassificationName { get; set; }
        [DisplayName("商品细分类图片地址")]
        public string GoodsClassificationImageUrl { get; set; }
        public ICollection<GoodsInfo> GoodsClassCode { get; set; }
        public int GoodsCommoditiesCode { get; set; }//一级分类id作为外键id
    }
}
