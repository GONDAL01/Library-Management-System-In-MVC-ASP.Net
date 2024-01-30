namespace Project5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBookReturnProperties : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookReturns",
                c => new
                    {
                        BookReturnId = c.Int(nullable: false, identity: true),
                        BorrowingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookReturnId)
                .ForeignKey("dbo.Borrowings", t => t.BorrowingId, cascadeDelete: true)
                .Index(t => t.BorrowingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookReturns", "BorrowingId", "dbo.Borrowings");
            DropIndex("dbo.BookReturns", new[] { "BorrowingId" });
            DropTable("dbo.BookReturns");
        }
    }
}
