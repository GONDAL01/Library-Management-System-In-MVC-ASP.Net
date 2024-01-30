using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project5.Models
{
    public class BookReturn
    {
        [Key]
        public int BookReturnId { get; set; }

        [Required]
        public int BorrowingId { get; set; }

        [ForeignKey("BorrowingId")]
        public virtual Borrowing Borrowing { get; set; }

    }
}