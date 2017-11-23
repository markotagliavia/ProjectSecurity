using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public enum AuditEventTypes
    {
        AuthenticationSuccess = 0,
        AuthorizationSuccess = 1,
        AuthorizationFailed = 2,
        RegistrationSuccess = 3,
        RegistrationFailed = 4,
        ChangePasswordSuccess = 5,
        ChangePasswordFailed = 6,
        ResetPasswordSuccess = 7,
        ResetPasswordFailed = 8,
        LogInSuccess = 9,
        LogInFailed = 10,
        LogOutSuccess = 11,
        LogOutFailed = 12,
        AddAdminSuccess = 13,
        AddAdminFailed = 14,
        DeleteAdminSuccess = 15,
        DeleteAdminfailed = 16,
        BlockUserSuccess = 17,
        BlockUserFailed = 18,
        RemoveBlockUserSuccess = 19,
        RemoveBlockUserFailed = 20,
        BlockGroupChatSuccess = 21,
        BlockGroupChatFailed = 22,
        RemoveBlockGroupChatSuccess = 23,
        RemoveBlockGroupChatFailed = 24,
        BlockUserFromRoomSuccess = 25,
        BlockUserFromRoomFailed = 26,
        RemoveBlockUserFromRoomSuccess = 27,
        RemoveBlockUserFromRoomFailed = 28,
        CreatePrivateChatSuccess = 29,
        CreatePrivateChatFailed = 30,
        CreateRoomSuccess = 31,
        CreateRoomFailed = 32,
        SendVerificationKeySuccess = 33,
        SendVerificationKeyFailed = 34,
        SendGroupMessageSuccess = 35,
        SendGroupMessageFailed = 36,
        SendRoomMessageSuccess = 37,
        SendRoomMessageFailed = 38,
        SendPrivateMessageSuccess = 39,
        SendPrivateMessageFailed = 40,
        CloseRoomSuccess = 41,
        CloseRoomFailed = 42,
        ModifiedMessageDanger = 43,
        BadAesKey = 44
    }

    public class Audit : IDisposable
    {
        private static EventLog customLog = null;
        const string SourceName = "SecurityManager.Audit";
        const string LogName = "MySecTest5";

        static Audit()
        {
            try
            {
                /// create customLog handle
                if (!System.Diagnostics.EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }
                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }

        public static void AuthenticationSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully authenticated.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.AuthenticationSuccess}) to event log."));
            }
        }

        public static void AuthorizationSuccess(string userName, string serviceName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully accessed to {serviceName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.AuthorizationSuccess}) to event log."));
            }
        }

        public static void AuthorizationFailed(string userName, string serviceName, string reason)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} Authorization failed. User {userName} failed to access {serviceName}. Reason: {reason}");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.AuthorizationFailed));
            }
        }

        public static void RegistrationSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully registrated.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RegistrationSuccess}) to event log."));
            }
        }

        public static void RegistrationFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} registration failed.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RegistrationSuccess}) to event log."));
            }
        }

        public static void ChangePasswordSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully changed password.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.ChangePasswordSuccess}) to event log."));
            }
        }

        public static void ChangePasswordFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to change password.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.ChangePasswordFailed}) to event log."));
            }
        }

        public static void ResetPasswordSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} reset password successfully .");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.ResetPasswordSuccess}) to event log."));
            }
        }

        public static void ResetPasswordFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to reset password.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.ResetPasswordFailed}) to event log."));
            }
        }

        public static void LogInSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully logged in.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.LogInSuccess}) to event log."));
            }
        }

        public static void LogInFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to log in.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.LogInFailed}) to event log."));
            }
        }

        public static void LogOutSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully logged out.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.LogOutSuccess}) to event log."));
            }
        }

        public static void LogOutFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to log out.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.LogOutFailed}) to event log."));
            }
        }

        public static void AddAdminSuccess(string userName, string adminName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully added admin rights to user {adminName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.AddAdminSuccess}) to event log."));
            }
        }

        public static void AddAdminFailed(string userName, string adminName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to add admin rights to user {adminName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.AddAdminFailed}) to event log."));
            }
        }

        public static void DeleteAdminSuccess(string userName, string adminName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} deleted admin rights to user {adminName} successfully.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.DeleteAdminSuccess}) to event log."));
            }
        }

        public static void DeleteAdminfailed(string userName, string adminName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to remove admin rights to user {adminName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.DeleteAdminfailed}) to event log."));
            }
        }

        public static void BlockUserSuccess(string userName, string blockUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully blocked user {blockUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.BlockUserSuccess}) to event log."));
            }
        }

        public static void BlockUserFailed(string userName, string blockUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to block user {blockUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.BlockUserFailed}) to event log."));
            }
        }

        public static void RemoveBlockUserSuccess(string userName, string blockUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} unblockd user {blockUserName} successfully.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockUserSuccess}) to event log."));
            }
        }

        public static void RemoveBlockUserFailed(string userName, string blockUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to unblock user {blockUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockUserFailed}) to event log."));
            }
        }

        public static void BlockGroupChatSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully blocked from group chat.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockUserSuccess}) to event log."));
            }
        }

        public static void BlockGroupChatFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Failed to block user {userName} from group chat.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.BlockGroupChatFailed}) to event log."));
            }
        }

        public static void RemoveBlockGroupChatSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} unblocked from group chat successfully.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockGroupChatSuccess}) to event log."));
            }
        }

        public static void RemoveBlockGroupChatFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Failed to unblock user {userName} from group chat.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockGroupChatFailed}) to event log."));
            }
        }

        public static void BlockUserFromRoomSuccess(string userName, string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} blocked from room {roomName} successfully.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.BlockUserFromRoomSuccess}) to event log."));
            }
        }

        public static void BlockUserFromRoomFailed(string userName, string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Failed to block user {userName} from room {roomName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.BlockUserFromRoomFailed}) to event log."));
            }
        }

        public static void RemoveBlockUserFromRoomSuccess(string userName, string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} unblocked from room {roomName} successfully.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockUserFromRoomSuccess}) to event log."));
            }
        }

        public static void RemoveBlockUserFromRoomFailed(string userName, string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Failed to unblock user {userName} from room {roomName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.RemoveBlockUserFromRoomFailed}) to event log."));
            }
        }

        public static void CreatePrivateChatSuccess(string firstUserName, string secondUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Private chat succesfully created between user {firstUserName} and user {secondUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.CreatePrivateChatSuccess}) to event log."));
            }
        }

        public static void CreatePrivateChatFailed(string firstUserName, string secondUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Private chat failde to create between user {firstUserName} and user {secondUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.CreatePrivateChatFailed}) to event log."));
            }
        }

        public static void CreateRoomSuccess(string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Room {roomName} successfully created.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.CreateRoomSuccess}) to event log."));
            }
        }

        public static void CreateRoomFailed(string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Room {roomName} failed to close.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.CreateRoomFailed}) to event log."));
            }
        }

        public static void SendVerificationKeySuccess()
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Verification key has been send successfully.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendVerificationKeySuccess}) to event log."));
            }
        }

        public static void SendVerificationKeyFailed()
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Failed to send verification key.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendVerificationKeyFailed}) to event log."));
            }
        }

        public static void SendGroupMessageSuccess(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully send message to group chat.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendGroupMessageSuccess}) to event log."));
            }
        }

        public static void SendGroupMessageFailed(string userName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to send message to group chat.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendGroupMessageFailed}) to event log."));
            }
        }

        public static void SendRoomMessageSuccess(string userName, string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} successfully send message to room {roomName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendRoomMessageSuccess}) to event log."));
            }
        }

        public static void SendRoomMessageFailed(string userName,string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {userName} failed to send message to room {roomName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendRoomMessageFailed}) to event log."));
            }
        }

        public static void SendPrivateMessageSuccess(string sendUserName, string recvUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {sendUserName} successfully send message to user {recvUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendPrivateMessageSuccess}) to event log."));
            }
        }

        public static void SendPrivateMessageFailed(string sendUserName, string recvUserName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | User {sendUserName} failed to send message to user {recvUserName}.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.SendPrivateMessageFailed}) to event log."));
            }
        }

        public static void CloseRoomSuccess(string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Room {roomName} successfully closed.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.CloseRoomSuccess}) to event log."));
            }
        }

        public static void CloseRoomFailed(string roomName)
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Room {roomName} failed to close.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.CloseRoomFailed}) to event log."));
            }
        }

        public static void ModifiedMessageDanger()
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Danger! Data sent through network are modified.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.ModifiedMessageDanger}) to event log."));
            }
        }

        public static void BadAesKey()
        {
            if (customLog != null)
            {
                string message = String.Format($"{DateTime.Now} | Danger! Application is under hacker attack. Please be carefull.");
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format($"{DateTime.Now} | Error while trying to write event (eventid = {(int)AuditEventTypes.BadAesKey}) to event log."));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
