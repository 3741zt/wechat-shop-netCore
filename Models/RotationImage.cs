using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
namespace ztbiyesheji.Models
{
    public class RotationImage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("轮播图主码")]
        public int RotationImageCode { get; set; }
        [DisplayName("轮播图图片地址")]
        public string RotationImageUrl { get; set; }//轮播图图片地址
        [DisplayName("轮播图跳转地址")]
        public string RotationImageJumpUrl { get; set; }//轮播图跳转地址
    }
}
