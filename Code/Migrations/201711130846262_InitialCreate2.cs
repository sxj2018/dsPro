namespace Code.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sys_Role",
                c => new
                    {
                        RoleCode = c.String(nullable: false, maxLength: 100),
                        RoleSeq = c.String(maxLength: 10),
                        RoleName = c.String(maxLength: 200),
                        Description = c.String(maxLength: 2048),
                        CreateTime = c.DateTime(),
                        CreateUser = c.String(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.String(),
                    })
                .PrimaryKey(t => t.RoleCode);
            
            CreateTable(
                "dbo.Sys_UserRoleMap",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserCode = c.String(maxLength: 10),
                        RoleCode = c.String(maxLength: 200),
                        CreateTime = c.DateTime(),
                        CreateUser = c.String(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sys_UserRoleMap");
            DropTable("dbo.Sys_Role");
        }
    }
}
