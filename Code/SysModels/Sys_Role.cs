using Code.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Code.SysModels
{
    public class Sys_Role:BaseModel
    {
        [Key]
        [Display(Name = "角色编号")]
        [StringLength(100)]
        public string RoleCode { get; set; }


        [Display(Name = "角色序列")]
        [StringLength(10)]
        public string RoleSeq { get; set; }

        [Display(Name = "角色名称")]
        [StringLength(200)]
        public string RoleName { get; set; }

        [Display(Name = "描述")]
        [StringLength(2048)]
        public string Description { get; set; }

         
    }
}