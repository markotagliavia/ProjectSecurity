
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ForumModels;
using System.Collections.ObjectModel;
using SecurityManager;

namespace Contracts
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IService
    {

        [OperationContract]
        bool Registration(string name, string sname, DateTime date, string gender, string email, string password);

        [OperationContract]
        bool ChangePassword(string email, string oldPassowrd, string newPassword);

        [OperationContract]
        int ResetPassword(string email);

        [OperationContract]
        int LogIn(string email, string password);

        [OperationContract]
        bool LogOut(string email);

        [OperationContract]
        bool AddAdmin(string email);

        [OperationContract]
        bool DeleteAdmin(string email);

        [OperationContract]
        bool BlockUser(string requestEmail, string blockEmail);

        [OperationContract]
        bool RemoveBlockUser(string requestEmail, string unblockEmail);

        [OperationContract]
        bool BlockGroupChat(string blockEmai);

        [OperationContract]
        bool RemoveBlockGroupChat(string unblockEmail);

        [OperationContract]
        bool BlockUserFromRoom(string blockEmail, string roomName);

        [OperationContract]
        bool RemoveBlockUserFromRoom(string unblockEmail, string roomName);

        [OperationContract]
        PrivateChat CreatePrivateChat(string firstEmail, string secondEmail);

        [OperationContract]
        PrivateChat GetPrivateChat(Guid code);

        [OperationContract]
        bool CreateRoom(string roomName);

        [OperationContract]
        bool SendVerificationKey(string key);

        [OperationContract]
        bool SendPrivateMessage(string sendEmail, string reciveEmail, string message);

        [OperationContract(IsOneWay = true)]
        void SendGroupMessage(string sender, string message);

        [OperationContract]
        bool SendRoomMessage(string sender, string roomName, string message);

        [OperationContract]
        GroupChat GetGroupChat();

        [OperationContract]
        Room GetThemeRoom(string roomName);

        [OperationContract]
        bool CloseRoom(string roomName);

        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract(IsOneWay = true)]
        void Subscribe(string email);


        [OperationContract(IsOneWay = true)]
        void SubscribeAllUsers(string email);

        [OperationContract]
        ObservableCollection<User> GetAllUsers();
    }

   
}
