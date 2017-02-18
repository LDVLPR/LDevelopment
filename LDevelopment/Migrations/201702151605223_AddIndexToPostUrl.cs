namespace LDevelopment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexToPostUrl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Posts", "Url", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.Posts", "Url", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Posts", new[] { "Url" });
            AlterColumn("dbo.Posts", "Url", c => c.String(nullable: false));
        }
    }
}
