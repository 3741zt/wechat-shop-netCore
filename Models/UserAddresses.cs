using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ztbiyesheji.Models
{
    public class UserAddresses
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("用户收货地址主码")]
        public int UserAddressesCode { get; set; }
        [DisplayName("用户姓名")]
        public string UserAddressesName { get; set; }
        [DisplayName("用户电话")]
        public string UserAddressesPhoneNum { get; set; }
        [DisplayName("详细地址")]
        public string UserDetailAddresses { get; set; }
        [DisplayName("收货省份")]
        public string UserAddressesProvince { get; set; }
        [DisplayName("收获城市")]
        public string UserAddressesCity { get; set; }
        [DisplayName("收货街道")]
        public string UserAddressesCountry { get; set; }
        [DisplayName("收货人")]
        public int UserAppCode { get; set; }
    }
}
