using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public enum Permissions
    {
        //TO DO
        Login = 0,
        Logout = 1,
    }

    public enum Roles
    {
        Admin = 0,
        User = 1,
    }

    class RolesConfiguration
    {
        static string[] AppAdminPermissions = new string[] { Permissions.Login.ToString(), Permissions.Logout.ToString() };
        static string[] UserPermissions = new string[] { Permissions.Login.ToString(), Permissions.Logout.ToString() };
        static string[] Empty = new string[] { };

        public static string[] GetPermissions(string role)
        {

            switch (role)
            {
                case "Admin": return AppAdminPermissions;
                case "User": return UserPermissions;
                default: return Empty;
            }
        }
    }
}
