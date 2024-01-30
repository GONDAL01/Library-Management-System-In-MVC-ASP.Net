namespace Project5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActualReturnDateToBorrowings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Borrowings", "ActualReturnDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Borrowings", "ActualReturnDate");
        }
    }
}
