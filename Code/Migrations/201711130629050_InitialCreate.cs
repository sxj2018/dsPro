namespace Code.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sys_RefreshToken",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sys_User",
                c => new
                    {
                        UserCode = c.String(nullable: false, maxLength: 100),
                        UserSeq = c.String(maxLength: 10),
                        UserName = c.String(maxLength: 200),
                        Description = c.String(maxLength: 2048),
                        Password = c.String(maxLength: 40),
                        RoleName = c.String(maxLength: 1000),
                        OrganizeName = c.String(maxLength: 1000),
                        ConfigJSON = c.String(maxLength: 4000),
                        IsEnable = c.Int(),
                        LoginCount = c.Int(),
                        LastLoginDate = c.DateTime(),
                        CreateTime = c.DateTime(),
                        CreateUser = c.String(),
                        UpdateTime = c.DateTime(),
                        UpdateUser = c.String(),
                    })
                .PrimaryKey(t => t.UserCode);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sys_User");
            DropTable("dbo.Sys_RefreshToken");
        }
    }
}
