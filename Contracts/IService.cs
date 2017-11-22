﻿
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
        bool ChangePassword(string email, string oldPassowrd, string newPassword);

        [OperationContract]
        int ResetPassword(string email);

        [OperationContract]
        int LogIn(byte[] email, byte[] password, string emailHash, string passwordHash);

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

        [OperationContract(IsOneWay =true)]
        void CreateRoom(string roomName);

        [OperationContract(IsOneWay = true)]
        void LeaveRoom(string theme);

        [OperationContract(IsOneWay = true)]
        void LeavePrivateChat(Guid code);

        [OperationContract]
        bool SendVerificationKey(string key);

        [OperationContract]
        bool SendPrivateMessage(string sendEmail, string reciveEmail, string message);

        [OperationContract(IsOneWay = true)]
        void SendGroupMessage(string sender, string message);

        [OperationContract(IsOneWay = true)]
        void SendRoomMessage(string sender, string roomName, string message);

        [OperationContract]
        GroupChat GetGroupChat();

        [OperationContract]
        Room GetThemeRoom(string roomName,string email);

        [OperationContract]
        bool CloseRoom(string roomName);

        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract(IsOneWay = true)]
        void Subscribe(string email);


        [OperationContract(IsOneWay = true)]
        void SubscribeAllUsers(string email);

        [OperationContract(IsOneWay = true)]
        void SubscribeUserTheme(string email, string theme);

        [OperationContract]
        ObservableCollection<User> GetAllUsers(string email);

        [OperationContract(IsOneWay = true)]
        void Unsubscribe(string email);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeUserTheme(string email, string theme);

        [OperationContract(IsOneWay = true)]
        void UnsubscribeAllUsers(string email);

        [OperationContract(IsOneWay = true)]
        void LogInTheme(string theme,string email);

        [OperationContract(IsOneWay = true)]
        void LogInPrivateChat(Guid code);

        [OperationContract]
        RSAParameters GetPublicKey(Guid code);

        [OperationContract]
        bool SendSessionKey(byte[] crypted);

        [OperationContract(IsOneWay = true)]
        void SendSessionKey(Guid code);
    }

   
}
