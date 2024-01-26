using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Project5.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; }

        [Required, StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required, StringLength(200)]
        public string Address { get; set; }

        [Required, StringLength(200)]
        public string PasswordHash { get; set; }
        public virtual ICollection<UserRole> UsersRoles { get; set;}
    }
}