using Code.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Code.SysModels
{
    public class Sys_Organize : BaseModel
    {
         	 
 
 
        [Key]
        [Display(Name = "组织编号")]
        [StringLength(100)]
        public string OrganizeCode { get; set; }


        [Display(Name = "上级组织编号")]
        [StringLength(100)]
        public string ParentCode { get; set; }

        [Display(Name = "排序")]
        [StringLength(10)]
        public string OrganizeSeq { get; set; }

        [Display(Name = "组织名称")]
        [StringLength(200)]
        public string OrganizeName { get; set; }

        [Display(Name = "描述")]
        [StringLength(2048)]
        public string Description { get; set; }

        
         

    }
}