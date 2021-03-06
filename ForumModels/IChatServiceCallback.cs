﻿using SecurityManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ForumModels
{
         public interface IChatServiceCallback
        {
            /// <summary>
            /// Implemented by the client so that the server may call
            /// this when it receives a message to be broadcasted.
            /// </summary>
            /// <param name="message">
            /// The message to broadcast.
            /// </param>
            [OperationContract(IsOneWay = true)]
            void HandleGroupChat(ForumModels.GroupChat gr);
            [OperationContract(IsOneWay = true)]
            void AllUsers(ObservableCollection<User> users);
            [OperationContract(IsOneWay = true)]
            void GetRoom(Room room);
            [OperationContract(IsOneWay = true)]
            void GetPrivateChat(PrivateChat pc);
    }
    
}
