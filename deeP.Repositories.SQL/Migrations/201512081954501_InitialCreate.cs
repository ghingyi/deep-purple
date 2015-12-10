namespace deeP.Repositories.SQL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bid",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Owner = c.String(nullable: false, maxLength: 256),
                        Title = c.String(nullable: false, maxLength: 256),
                        Price = c.Double(nullable: false),
                        State = c.Byte(nullable: false),
                        PropertyId = c.String(nullable: false, maxLength: 36),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .Index(t => t.Owner)
                .Index(t => t.PropertyId);
            
            CreateTable(
                "dbo.Property",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Owner = c.String(nullable: false, maxLength: 256),
                        Description = c.String(nullable: false, maxLength: 1000),
                        Type = c.Byte(nullable: false),
                        Bedrooms = c.Byte(nullable: false),
                        Price = c.Double(nullable: false),
                        Address = c.String(nullable: false, maxLength: 300),
                        LocationDetails = c.String(nullable: false),
                        State = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Owner);
            
            CreateTable(
                "dbo.ImageInfo",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 36),
                        Uri = c.String(nullable: false, maxLength: 2048),
                        Title = c.String(nullable: false, maxLength: 160),
                        Order = c.Int(nullable: false),
                        PropertyId = c.String(nullable: false, maxLength: 36),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Property", t => t.PropertyId, cascadeDelete: true)
                .Index(t => t.PropertyId);
            
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageData = c.Binary(storeType: "image"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bid", "PropertyId", "dbo.Property");
            DropForeignKey("dbo.ImageInfo", "PropertyId", "dbo.Property");
            DropIndex("dbo.ImageInfo", new[] { "PropertyId" });
            DropIndex("dbo.Property", new[] { "Owner" });
            DropIndex("dbo.Bid", new[] { "PropertyId" });
            DropIndex("dbo.Bid", new[] { "Owner" });
            DropTable("dbo.Image");
            DropTable("dbo.ImageInfo");
            DropTable("dbo.Property");
            DropTable("dbo.Bid");
        }
    }
}
