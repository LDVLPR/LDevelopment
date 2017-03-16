namespace LDevelopment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostUrlMaxLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Posts", "Url", c => c.String(nullable: false, maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Posts", "Url", c => c.String(nullable: false));
        }
    }
}
