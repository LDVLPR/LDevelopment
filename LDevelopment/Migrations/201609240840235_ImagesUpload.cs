namespace LDevelopment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ImagesUpload : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Posts", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Posts", "Image");
        }
    }
}
