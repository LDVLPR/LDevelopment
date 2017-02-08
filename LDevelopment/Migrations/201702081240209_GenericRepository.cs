namespace LDevelopment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GenericRepository : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Logs", "DeletedDate", c => c.DateTime());
            AddColumn("dbo.Posts", "Url", c => c.String(nullable: false));
            AddColumn("dbo.Posts", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Posts", "DeletedDate", c => c.DateTime());
            AddColumn("dbo.Comments", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Comments", "DeletedDate", c => c.DateTime());
            AddColumn("dbo.Tags", "IsDeleted", c => c.Boolean());
            AddColumn("dbo.Tags", "DeletedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Tags", "DeletedDate");
            DropColumn("dbo.Tags", "IsDeleted");
            DropColumn("dbo.Comments", "DeletedDate");
            DropColumn("dbo.Comments", "IsDeleted");
            DropColumn("dbo.Posts", "DeletedDate");
            DropColumn("dbo.Posts", "IsDeleted");
            DropColumn("dbo.Posts", "Url");
            DropColumn("dbo.Logs", "DeletedDate");
            DropColumn("dbo.Logs", "IsDeleted");
        }
    }
}
