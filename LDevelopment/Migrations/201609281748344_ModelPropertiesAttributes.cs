namespace LDevelopment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModelPropertiesAttributes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Comments", "Post_Id", "dbo.Posts");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.Users");
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropIndex("dbo.Comments", new[] { "Post_Id" });
            RenameColumn(table: "dbo.Comments", name: "Post_Id", newName: "PostId");
            AlterColumn("dbo.Comments", "AuthorId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Comments", "PostId", c => c.Int(nullable: false));
            CreateIndex("dbo.Comments", "AuthorId");
            CreateIndex("dbo.Comments", "PostId");
            AddForeignKey("dbo.Comments", "PostId", "dbo.Posts", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Comments", "AuthorId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Comments", "PostId", "dbo.Posts");
            DropIndex("dbo.Comments", new[] { "PostId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            AlterColumn("dbo.Comments", "PostId", c => c.Int());
            AlterColumn("dbo.Comments", "AuthorId", c => c.String(maxLength: 128));
            RenameColumn(table: "dbo.Comments", name: "PostId", newName: "Post_Id");
            CreateIndex("dbo.Comments", "Post_Id");
            CreateIndex("dbo.Comments", "AuthorId");
            AddForeignKey("dbo.Comments", "AuthorId", "dbo.Users", "Id");
            AddForeignKey("dbo.Comments", "Post_Id", "dbo.Posts", "Id");
        }
    }
}
