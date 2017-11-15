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
        public void ChangePassword(string oldPassowrd, string newPassword)
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

        public void ResetPassword(string email)
        {
            throw new NotImplementedException();
        }
    }
}
