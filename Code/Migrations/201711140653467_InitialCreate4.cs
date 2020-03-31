namespace Code.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sys_Organize",
                c => new
                    {
                        OrganizeCode = c.String(nullable: false, maxLength: 100),
                        ParentCode = c.String(maxLength: 100),
                        OrganizeSeq = c.String(maxLength: 10),
                        OrganizeName = c.String(maxLength: 200),
                        Description = c.String(maxLength: 2048),
                        CreateTime = c.DateTime(),
                        CreateUser = c.String(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.String(),
                    })
                .PrimaryKey(t => t.OrganizeCode);
            
            CreateTable(
                "dbo.Sys_OrganizeRoleMap",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrganizeCode = c.String(maxLength: 100),
                        RoleCode = c.String(maxLength: 100),
                        CreateTime = c.DateTime(),
                        CreateUser = c.String(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Sys_UserOrganizeMap",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OrganizeCode = c.String(maxLength: 100),
                        UserCode = c.String(maxLength: 100),
                        CreateTime = c.DateTime(),
                        CreateUser = c.String(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sys_UserOrganizeMap");
            DropTable("dbo.Sys_OrganizeRoleMap");
            DropTable("dbo.Sys_Organize");
        }
    }
}
