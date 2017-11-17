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
        LogIn = 0,
        LogOut = 1,
        Registration = 2,
        ChangePassword = 3,
        ResetPassword = 4,
        AddAdmin = 5,
        DeleteAdmin = 6,
        BlockUser = 7,
        RemoveBlockUser = 8,
        BlockGroupChat = 9,
        RemoveBlockGroupChat = 10,
        BlockUserFromRoom = 11,
        RemoveBlockRoom = 12,
        CreateRoom = 13,
        CreatePrivateChat = 14,
        SendVerificationKey = 15,
        SendPrivateMessage = 16,
        SendGroupMessage = 17,
        SendRoomMessage = 18,
        GetGroupChat = 19,
        GetPrivateRoom = 20,
        CloseRoom = 21,
    }

    public enum Roles
    {
        Admin = 0,
        User = 1,
    }

    class RolesConfiguration
    {
        static string[] AppAdminPermissions = new string[] { Permissions.GetPrivateRoom.ToString(), Permissions.GetGroupChat.ToString(), Permissions.SendRoomMessage.ToString(), Permissions.SendGroupMessage.ToString(), Permissions.SendPrivateMessage.ToString(), Permissions.SendVerificationKey.ToString(), Permissions.LogIn.ToString(), Permissions.LogOut.ToString(), Permissions.Registration.ToString(), Permissions.ChangePassword.ToString(), Permissions.ResetPassword.ToString(), Permissions.AddAdmin.ToString(), Permissions.DeleteAdmin.ToString(), Permissions.BlockGroupChat.ToString(), Permissions.BlockUser.ToString(), Permissions.RemoveBlockUser.ToString(), Permissions.BlockUserFromRoom.ToString(), Permissions.RemoveBlockRoom.ToString(), Permissions.CreateRoom.ToString(), Permissions.CreatePrivateChat.ToString(), Permissions.CloseRoom.ToString() };
        static string[] UserPermissions = new string[] { Permissions.GetPrivateRoom.ToString(), Permissions.GetGroupChat.ToString(), Permissions.SendRoomMessage.ToString(), Permissions.SendGroupMessage.ToString(), Permissions.SendPrivateMessage.ToString(), Permissions.SendVerificationKey.ToString(), Permissions.LogIn.ToString(), Permissions.LogOut.ToString(), Permissions.Registration.ToString(), Permissions.ChangePassword.ToString(), Permissions.ResetPassword.ToString(), Permissions.BlockUser.ToString(), Permissions.RemoveBlockUser.ToString(), Permissions.CreateRoom.ToString(), Permissions.CreatePrivateChat.ToString() };
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
