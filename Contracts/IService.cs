
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
        bool Registration(byte[] nameBytes, byte[] snameBytes, byte[] dateBytes, byte[] genderBytes, byte[] emailBytes, byte[] passwordBytes, string nameHash, string snameHash, string dateHash, string genderHash, string emailHash, string passwordHash);

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

        [OperationContract(IsOneWay = true)]
        void BlockUser(byte[] requestEmailBytes, byte[] blokEmailBytes, string requestEmailHash, string blockEmailHash);

        [OperationContract(IsOneWay =true)]
        void RemoveBlockUser(byte[] requestEmailBytes, byte[] unblockEmailBytes, string requestEmailHash, string unblockEmailHash);

        [OperationContract(IsOneWay = true)]
        void BlockGroupChat(byte[] blockEmailBytes, string blockEmailHash);

        [OperationContract(IsOneWay = true)]
        void RemoveBlockGroupChat(byte[] unblockEmailBytes, string unblockEmailHash);

        [OperationContract(IsOneWay = true)]
        void BlockUserFromRoom(byte[] blokEmailBytes, byte[] roomNameBytes, string blockEmailHash, string roomNameHash);

        [OperationContract(IsOneWay = true)]
        void RemoveBlockUserFromRoom(byte[] unblockEmailBytes, byte[] roomNameBytes, string unblockEmailHash, string roomNameHash);

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

        [OperationContract(IsOneWay =true)]
        void CloseRoom(byte[] roomNameBytes, string roomNameHash);

        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract(IsOneWay = true)]
        void Subscribe(byte[] emailBytes, string emailHash);


        [OperationContract(IsOneWay = true)]
        void SubscribeAllUsers(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void SubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash);

        [OperationContract(IsOneWay = true)]
        void SubscribeUserChat(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash);

        [OperationContract]
        ObservableCollection<User> GetAllUsers(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeUserTheme(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeUserChat(byte[] emailBytes, byte[] themeBytes, string emailHash, string themeHash);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeAllUsers(byte[] emailBytes, string emailHash);

        [OperationContract(IsOneWay = true)]
        void LogInTheme(byte[] themeBytes, byte[] emailBytes, string themeHash, string emailHash);

        [OperationContract(IsOneWay = true)]
        void LogInPrivateChat(byte[] emailBytes, string emailHash, byte[] codeByte, string codeHash);

        [OperationContract]
        RSAParameters GetPublicKey(Guid code);

        [OperationContract]
        bool SendSessionKey(byte[] crypted);

        [OperationContract(IsOneWay = true)]
        void SessionKey(Guid code);
    }

   
}
