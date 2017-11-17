using Forum;
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
        void Registration(string name, string sname, DateTime date, string gender, string email, string password);

        [OperationContract]
        void ChangePassword(string oldPassowrd, string newPassword);

        [OperationContract]
        int ResetPassword(string email);

        [OperationContract]
        int LogIn(string email, string password);

        [OperationContract]
        void LogOut(string email);

        [OperationContract]
        bool AddAdmin(string email);

        [OperationContract]
        void DeleteAdmin(string email);

        [OperationContract]
        void BlockUser(string requestEmail, string blockEmail);

        [OperationContract]
        void RemoveBlockUser(string requestEmail, string unblockEmail);

        [OperationContract]
        bool BlockGroupChat(string blockEmai);

        [OperationContract]
        void RemoveBlockGroupChat(string unblockEmail);

        [OperationContract]
        bool BlockUserFromRoom(string blockEmail, string roomName);

        [OperationContract]
        bool RemoveBlockUserFromRoom(string unblockEmail, string roomName);

        [OperationContract]
        void CreatePrivateChat(string firstEmail, string secondEmail);

        [OperationContract]
        bool CreateRoom(string roomName);

        [OperationContract]
        bool SendVerificationKey(string key);

        [OperationContract]
        bool SendPrivateMessage(string firstEmail, string secondEmail, string message);

        [OperationContract]
        bool SendGroupMessage(string email, string message);

        [OperationContract]
        bool SendRoomMessage(string email, string roomName, string message);

        [OperationContract]
        GroupChat GetGroupChat();

        [OperationContract]
        Room GetPrivateRoom(string roomName);

        [OperationContract]
        bool CloseRoom(string roomName, string email);
    }
}
