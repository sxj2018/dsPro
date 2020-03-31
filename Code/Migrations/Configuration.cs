namespace Code.Migrations
{
    using Code.Initial;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Code.DbContexts.SysDbServiceContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Code.DbContexts.SysDbServiceContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            #region 初始化数据

            Initialize.ModelInit(context);

            #endregion
        }
    }
}
