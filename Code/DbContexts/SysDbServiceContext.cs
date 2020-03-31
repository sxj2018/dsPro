using Code.SysModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Code.DbContexts
{
    public class SysDbServiceContext:DbContext
    {
        public SysDbServiceContext(): base("SysContext")
        {
            
        }

        #region 系统表T_Sys
        public virtual DbSet<Sys_User> Sys_User { get; set; }
        public virtual DbSet<Sys_RefreshToken> Sys_RefreshToken { get; set; }
        public virtual DbSet<Sys_Role> Sys_Role { get; set; }
        public virtual DbSet<Sys_UserRoleMap> Sys_UserRoleMap { get; set; }
        public virtual DbSet<Sys_Organize> Sys_Organize { get; set; }
        public virtual DbSet<Sys_OrganizeRoleMap> Sys_OrganizeRoleMap { get; set; }
        public virtual DbSet<Sys_UserOrganizeMap> Sys_UserOrganizeMap { get; set; }
        #endregion
    }
}