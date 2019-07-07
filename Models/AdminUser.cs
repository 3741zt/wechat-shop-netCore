using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static ztbiyesheji.Models.ENUM.Enum;

namespace ztbiyesheji.Models
{
    public class AdminUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("管理员主码")]
        public string AdminUserCode { get; set; }
        [DisplayName("管理员账号")]
        public string AdminUserNumber { get; set; }
        [DisplayName("管理员密码")]
        public string AdminUserPwd { get; set; }
        [DisplayName("管理员姓名")]
        public string AdminUserName { get; set; }
        [DisplayName("管理员电话号码")]
        public string AdminUserPhoneNumber { get; set; }
        [DisplayName("管理员性别")]
        public AdminUserSexes AdminUserSex { get; set; }
        [DisplayName("管理员头像")]
        public string AdminImage { get; set; }
        [DisplayName("管理员权限")]
        public AdminUserRoleType RoleType { get; set; }

    }
}
