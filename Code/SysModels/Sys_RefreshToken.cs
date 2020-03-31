using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Code.SysModels
{
    public class Sys_RefreshToken
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }
        public DateTime IssuedUtc { get; set; }
        public DateTime ExpiresUtc { get; set; }
        [Required]
        public string ProtectedTicket { get; set; }
    }
}