namespace Code.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Sys_UserRoleMap", "UserCode", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sys_UserRoleMap", "UserCode", c => c.String(maxLength: 10));
        }
    }
}
