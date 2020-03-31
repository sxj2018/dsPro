using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Code.Models
{
    public class BaseModel
    {
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { set; get; }

        [Display(Name = "创建人")]
        public String CreateUser { set; get; }

        [Display(Name = "更新时间")]
        public DateTime? UpdateTime { set; get; }
        [Display(Name = "更新人")]
        public String UpdateUser { set; get; }

        /// <summary>
        /// 页索引
        /// </summary>
        [NotMapped]
        [Display(Name = "页索引")]
        public int PageIndex { set; get;  } 
        /// <summary>
        /// 页大小
        /// 
        /// </summary>
        /// 
        [Display(Name = "页大小")]
        [NotMapped]
        public int PageSize { set; get;  }

        [Display(Name = "开始时间")]
        [NotMapped]
        public DateTime? StartTime { set; get; }

        [Display(Name = "结束时间")]
        [NotMapped]
        public DateTime? EndTime { set; get; }
    }
}