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

        [OperationContract]
        void AddAdmin(string email);

        [OperationContract]
        void DeleteAdmin(string email);

        [OperationContract]
        void BlockUser(string requestEmail, string blockEmail);

        [OperationContract]
        void RemoveBlockUser(string requestEmail, string unblockEmail);

        [OperationContract]
        void BlockGroupChat(string blockEmai);

        [OperationContract]
        void RemoveBlockGroupChat(string unblockEmail);

        [OperationContract]
        void BlockUserFromRoom(string blockEmail, string roomName);

        [OperationContract]
        void RemoveBlockUserFromRoom(string unblockEmail, string roomName);

        [OperationContract]
        void CreatePrivateChat(string firstEmail, string secondEmail);

        [OperationContract]
        void CreateRoom(string roomName);

    }
}
