using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project5.Models
{
    public class Book
    {

        [Key]
        public int BookId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        public string Genre { get; set; }

        public int Quantity { get; set; }

        // Navigation property to track borrowings of this book
        public virtual ICollection<Borrowing> Borrowings { get; set; }
    }

    public enum Genre
    {
        Fiction, NonFiction, Biography, Science, History, Education
    }

}
