using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project5.Models
{
    public class Role
    {
        public int id {  get; set; }
        public string Rolename { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}