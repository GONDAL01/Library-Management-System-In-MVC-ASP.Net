using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project5.Models
{
    public class UserRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get { return "Project5"; } // Set your application name here.
            set { /* Do nothing or throw not supported exception if setting is not implemented. */ }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (var context = new Project5Context())
            {
                return context.Roles.Select(r => r.Rolename).ToArray();
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var context = new Project5Context())
            {
                var userRoles = (from user in context.Users
                                 where user.Email.Equals(username, System.StringComparison.OrdinalIgnoreCase)
                                 from role in user.UsersRoles
                                 select role.Role.Rolename).ToArray();
                return userRoles;
            }
        }
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var context = new Project5Context())
            {
                var user = context.Users.SingleOrDefault(u => u.Email.Equals(username, System.StringComparison.OrdinalIgnoreCase));

                if (user == null)
                    return false;

                return user.UsersRoles.Any(ur => ur.Role.Rolename.Equals(roleName, System.StringComparison.OrdinalIgnoreCase));
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            using (var context = new Project5Context())
            {
                return context.Roles.Any(r => r.Rolename.Equals(roleName, System.StringComparison.OrdinalIgnoreCase));
            }
        }
    }
}