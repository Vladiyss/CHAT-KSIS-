﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommonInfo
{
    public struct ClientsInfo
    {
        public int clientID;
        public string clientName;

        public override string ToString()
        {
            return clientName;
        }
    }

    public class Message
    {
        public static Dictionary<int, string> MessageType = new Dictionary<int, string>()
        {
            [1] = "Common",
            [2] = "Private",
            [3] = "History",
            [4] = "JoinToChat",
            [5] = "ClientsList",
            [6] = "SearchRequest",
            [7] = "SearchResponce"
        };

        public int clientPort;
        public int serverPort;
        public int messageSenderID;
        public int messageReceiverID;
        public string IPAdress;

        public string messageType;
        public string messageContent;
        public string messageName;

        public List<ClientsInfo> clientsInfo;
        public List<string> messageHistory;
        
        public DateTime messageTime;

        public Message(string iPAdress, int port, string messageType)
        {
            IPAdress = iPAdress;
            clientPort = port;
            this.messageType = messageType;
        }

        public Message(string data, string messageType)
        {
            this.messageType = messageType;

            if (this.messageType == MessageType[1])
            {
                messageContent = data;
                messageName = "";
            }
            else if (this.messageType == MessageType[4])
            {
                messageContent = "";
                messageName = data;
            }
        }

        public Message(int receiver, string content)
        {
            messageReceiverID = receiver;
            messageContent = content;
            messageName = "Me";
            messageType = MessageType[2];
        }

        public Message(List<ClientsInfo> clientsInfo)
        {
            this.clientsInfo = clientsInfo;
            messageType = MessageType[5];
        }

        public Message(string content)
        {
            messageType = MessageType[1];
            messageContent = content;
            messageName = "Кто-то";
        }

        public Message()
        {

        }
    }
}