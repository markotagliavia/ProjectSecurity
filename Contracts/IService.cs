using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void Registration(string name, string sname, DateTime date, char gender, string email, string password);

        [OperationContract]
        void ChangePassword(string oldPassowrd, string newPassword);

        [OperationContract]
        void ResetPassword(string email);

        [OperationContract]
        void LogIn(string email, string password, string code);

        [OperationContract]
        void LogOut(string email);
    }
}
