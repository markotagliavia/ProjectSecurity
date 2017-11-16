using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceApp
{
    public class Service : IService
    {
        public void AddAdmin(string email)
        {
            throw new NotImplementedException();
        }

        public void BlockGroupChat(string blockEmai)
        {
            throw new NotImplementedException();
        }

        public void BlockUser(string requestEmail, string blockEmail)
        {
            throw new NotImplementedException();
        }

        public void BlockUserFromRoom(string blockEmail, string roomName)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string oldPassowrd, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void CreatePrivateChat(string firstEmail, string secondEmail)
        {
            throw new NotImplementedException();
        }

        public void CreateRoom(string roomName)
        {
            throw new NotImplementedException();
        }

        public void DeleteAdmin(string email)
        {
            throw new NotImplementedException();
        }

        public void LogIn(string email, string password, string code)
        {
            throw new NotImplementedException();
        }

        public void LogOut(string email)
        {
            throw new NotImplementedException();
        }

        public void Registration(string name, string sname, DateTime date, char gender, string email, string password)
        {
            throw new NotImplementedException();
        }

        public void RemoveBlockGroupChat(string unblockEmail)
        {
            throw new NotImplementedException();
        }

        public void RemoveBlockUser(string requestEmail, string unblockEmail)
        {
            throw new NotImplementedException();
        }

        public void RemoveBlockUserFromRoom(string unblockEmail, string roomName)
        {
            throw new NotImplementedException();
        }

        public void ResetPassword(string email)
        {
            throw new NotImplementedException();
        }
    }
}
