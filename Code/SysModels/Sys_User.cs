using Code.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Utils.Attr;
using Utils.Enum;

namespace Code.SysModels
{
    public class Sys_User:BaseModel
    { 
         
  
  
        [Key]
        [Display(Name = "用户编号")] 
        [StringLength(100)]
        [LikeAttribute(Type = LikeType.EndsWith)]
        public string UserCode { get; set; }

        
        [Display(Name = "用户序列")] 
        [StringLength(10)]
        public string UserSeq { get; set; }

        [Display(Name = "用户名称")] 
        [StringLength(200)]
        public string UserName { get; set; }

        [Display(Name = "描述")] 
        [StringLength(2048)]
        public string Description { get; set; }

        [Display(Name = "密码")] 
        [StringLength(40)]
        public string Password { get; set; }

         [Display(Name = "角色名")] 
        [StringLength(1000)]
        public string RoleName { get; set; }

         [Display(Name = "组织名")]
         [StringLength(1000)]
         public string OrganizeName { get; set; }

         [Display(Name = "JSON")]
         [StringLength(4000)]
         public string ConfigJSON { get; set; }

         [Display(Name = "是否启用")]
         public int? IsEnable { get; set; }

         [Display(Name = "登陆次数")]
         public int? LoginCount { get; set; }


         [Display(Name = "最后登陆时间")]
         public DateTime? LastLoginDate { get; set; }

    }
}