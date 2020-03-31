using Code.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Code.SysModels
{
    public class Sys_UserOrganizeMap : BaseModel
    {
        [Key]
        [Display(Name = "ID")]
        //[DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None)]//不自动增长
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]//添加时自动增长

        public int ID { get; set; }


        [Display(Name = "组织编号")]
        [StringLength(100)]
        public string OrganizeCode { get; set; }

        [Display(Name = "用户编号")]
        [StringLength(100)]
        public string UserCode { get; set; }
    }
}