using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project5.Models
{
    public class BorrowingSummaryViewModel
    {
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}