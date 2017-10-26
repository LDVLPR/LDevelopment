namespace LDevelopment.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class NamingConventionChanges : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Posts", new[] { "Url" });
            RenameColumn(table: "dbo.PostTags", name: "PostModel_Id", newName: "Post_Id");
            RenameColumn(table: "dbo.PostTags", name: "TagModel_Id", newName: "Tag_Id");
            RenameIndex(table: "dbo.PostTags", name: "IX_PostModel_Id", newName: "IX_Post_Id");
            RenameIndex(table: "dbo.PostTags", name: "IX_TagModel_Id", newName: "IX_Tag_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.PostTags", name: "IX_Tag_Id", newName: "IX_TagModel_Id");
            RenameIndex(table: "dbo.PostTags", name: "IX_Post_Id", newName: "IX_PostModel_Id");
            RenameColumn(table: "dbo.PostTags", name: "Tag_Id", newName: "TagModel_Id");
            RenameColumn(table: "dbo.PostTags", name: "Post_Id", newName: "PostModel_Id");
            CreateIndex("dbo.Posts", "Url", unique: true);
        }
    }
}
