
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ForumModels;
using System.Collections.ObjectModel;
using SecurityManager;
using System.Security.Cryptography;

namespace Contracts
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IService
    {

        [OperationContract]
        bool Registration(string name, string sname, DateTime date, string gender, string email, string password);

        [OperationContract]
        bool ChangePassword(byte[] emailBytes, byte[] oldPasswordBytes, byte[] newPasswordBytes, string emailHash, string oldPassowrdHash, string newPasswordHash);

        [OperationContract]
        int ResetPassword(byte[] emailBytes, string emailHash);

        [OperationContract]
        int LogIn(byte[] email, byte[] password, string emailHash, string passwordHash);

        [OperationContract]
        bool LogOut(byte[] emailBytes, string emailHash);

        [OperationContract]
        bool AddAdmin(byte[] emailBytes, string emailHash);

        [OperationContract]
        bool DeleteAdmin(byte[] emailBytes, string emailHash);

        [OperationContract]
        bool BlockUser(byte[] requestEmailBytes, byte[] blokEmailBytes, string requestEmailHash, string blockEmailHash);

        [OperationContract]
        bool RemoveBlockUser(byte[] requestEmailBytes, byte[] unblockEmailBytes, string requestEmailHash, string unblockEmailHash);

        [OperationContract]
        bool BlockGroupChat(byte[] blockEmailBytes, string blockEmailHash);

        [OperationContract]
        bool RemoveBlockGroupChat(byte[] unblockEmailBytes, string unblockEmailHash);

        [OperationContract]
        bool BlockUserFromRoom(byte[] blokEmailBytes, byte[] roomNameBytes, string blockEmailHash, string roomNameHash);

        [OperationContract]
        bool RemoveBlockUserFromRoom(byte[] unblockEmailBytes, byte[] roomNameBytes, string unblockEmailHash, string roomNameHash);

        [OperationContract]
        PrivateChat CreatePrivateChat(byte[] firstEmailBytes, byte[] secondEmailBytes, string firstEmailHash, string secondEmailHash);

        [OperationContract]
        PrivateChat GetPrivateChat(byte[] codeByte, string codeHash);

        [OperationContract(IsOneWay =true)]
        void CreateRoom(byte[] roomNameBytes, string roomNameHash);

        [OperationContract(IsOneWay = true)]
        void LeaveRoom(byte[] themeBytes, string themeHash);

        [OperationContract(IsOneWay = true)]
        void LeavePrivateChat(byte[] codeByte, string codeHash);

        [OperationContract]
        bool SendVerificationKey(byte[] keyBytes, string keyHash);

        [OperationContract(IsOneWay = true)]
        void SendPrivateMessage(byte[] sendEmailBytes, byte[] reciveEmailBytes, byte[] messageBytes, string sendEmailHash, string reciveEmailHash, string messageHash);

        [OperationContract(IsOneWay = true)]
        void SendGroupMessage(byte[] userNameBytes, byte[] messageBytes, string userNameHash, string messageHash);

        [OperationContract(IsOneWay = true)]
        void SendRoomMessage(byte[] userNameBytes, byte[] roomNameBytes, byte[] messageBytes, string userNameHash, string roomNameHash, string messageHash);

        [OperationContract]
        GroupChat GetGroupChat();

        [OperationContract]
        Room GetThemeRoom(byte[] roomNameBytes, byte[] emailBytes, string roomNameHash, string emailHash);

        [OperationContract]
        bool CloseRoom(byte[] roomNameBytes, string roomNameHash);

        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract(IsOneWay = true)]
        void Subscribe(byte[] emailBytes, string emailHash);


        [OperationContract(IsOneWay = true)]
        void SubscribeAllUsers(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void SubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash);

        [OperationContract]
        ObservableCollection<User> GetAllUsers(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeAllUsers(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void LogInTheme(byte[] themeBytes, byte[] emailBytes, string themeHash, string emailHash);

        [OperationContract(IsOneWay = true)]
        void LogInPrivateChat(byte[] codeByte, string codeHash);

        [OperationContract]
        RSAParameters GetPublicKey(Guid code);

        [OperationContract]
        bool SendSessionKey(byte[] crypted);

        [OperationContract(IsOneWay = true)]
        void SendSessionKey(Guid code);
    }

   
}
