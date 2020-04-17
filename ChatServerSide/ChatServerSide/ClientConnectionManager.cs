using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using ChatCommonInfo;

namespace ChatServerSide
{
    class ClientConnectionManager
    {
        Socket listeningClientMessagesSocket;
        Thread handleClientThread;

        public bool isConnected;
        MessageSerializer messageSerializer;

        public ClientConnectionManager(Socket listeningClientMessagesSocket)
        {
            messageSerializer = new MessageSerializer();
            isConnected = true;
            this.listeningClientMessagesSocket = listeningClientMessagesSocket;
            listeningClientMessagesSocket.ReceiveTimeout = 700;
            listeningClientMessagesSocket.SendTimeout = 700;
            Server.clientSockets.Add(listeningClientMessagesSocket.RemoteEndPoint.GetHashCode(), listeningClientMessagesSocket);
            handleClientThread = new Thread(ListeningClient);
            handleClientThread.Start();
        }

        public void ListeningClient()
        {
            ReceiveClientMessages();
            DisconnectClient();
        }

        public void ReceiveClientMessages()
        {
            byte[] data = new byte[1024];
            int amount;
            do
            {
                MemoryStream messageContainer = new MemoryStream();
                try
                {
                    do
                    {
                        amount = listeningClientMessagesSocket.Receive(data);
                        messageContainer.Write(data, 0, amount);
                    } while (listeningClientMessagesSocket.Available > 0);
                    Message recievedMessage = messageSerializer.Deserialize(messageContainer.GetBuffer(),
                        messageContainer.GetBuffer().Length);
                    DefineMessageType(recievedMessage);
                }
                catch
                {
                    if (!IsClientConnected())
                    {
                        isConnected = false;
                    }
                }
            } while (isConnected);
        }

        int RetrieveMessageType(string messageType)
        {
            int i = 1;
            bool flag = true;
            while ((i <= 7) && flag)
            {
                int result = String.Compare(Message.MessageType[i], messageType);
                if (0 == result)
                    flag = false;
                else
                    ++i;
            }
            return i;
        }

        void DefineMessageType(Message message)
        {
            int i = RetrieveMessageType(message.messageType);

            message.messageTime = DateTime.Now;
            message.IPAdress = ((IPEndPoint)listeningClientMessagesSocket.RemoteEndPoint).Address.ToString();
            switch (i)
            {
                case 1:
                    ProcessCommonMessage(message);
                    break;
                case 2:
                    ProcessPrivateMessage(message);
                    break;
                case 3:
                    ProcessHistoryRequestMessage(message);
                    break;
                case 4:
                    ProcessJoinToChatMessage(message);
                    break;
            }
        }

        void ProcessCommonMessage(Message message)
        {
            Console.WriteLine(Server.clientNames[listeningClientMessagesSocket.RemoteEndPoint.GetHashCode()]
                       + " : " + message.messageContent);
            message.messageName = Server.clientNames[listeningClientMessagesSocket.RemoteEndPoint.GetHashCode()];
            Server.SendToAll(message);
            Server.MessageHistory.Add(message.messageName + " : " + message.messageContent
                + "\n" + (message.messageTime).ToString() + "\n" + message.IPAdress + "\n" + (message.clientPort).ToString());
        }

        void ProcessPrivateMessage(Message message)
        {
            message.messageName = Server.clientNames[listeningClientMessagesSocket.RemoteEndPoint.GetHashCode()];
            if (Server.clientSockets.ContainsKey(message.messageReceiverID))
            {
                message.messageSenderID = listeningClientMessagesSocket.RemoteEndPoint.GetHashCode();
                message.messageContent = Server.clientNames[message.messageSenderID] + " : " + message.messageContent;
                Server.SendMessage(message, Server.clientSockets[message.messageReceiverID]);
            }
            else
            {
                Console.WriteLine("Failed to send message");
            }
        }

        void ProcessHistoryRequestMessage(Message message)
        {
            Message responseMessage = new Message() { messageHistory = Server.MessageHistory, messageType = Message.MessageType[3] };
            Server.SendMessage(responseMessage, listeningClientMessagesSocket);
        }

        void ProcessJoinToChatMessage(Message message)
        {
            Server.clientNames.Add(listeningClientMessagesSocket.RemoteEndPoint.GetHashCode(), message.messageName);
            Console.WriteLine(message.messageName + " joined the chat");
            List<ClientsInfo> info = GetClientsList();
            Server.SendToAll(new Message(info));
        }

        public List<ClientsInfo> GetClientsList()
        {
            List<ClientsInfo> info = new List<ClientsInfo>();
            foreach (KeyValuePair<int, string> keyValuePair in Server.clientNames)
            {
                info.Add(new ClientsInfo() { clientID = keyValuePair.Key, clientName = keyValuePair.Value });
            }
            return info;
        }

        bool IsClientConnected()
        {
            bool IsConnected = true;
            try
            {
                if (!listeningClientMessagesSocket.Connected)
                    IsConnected = false;
            }
            catch
            {
                IsConnected = false;
            }
            return IsConnected;
        }

        public void RemoveClient()
        {
            Console.WriteLine(Server.clientNames[listeningClientMessagesSocket.RemoteEndPoint.GetHashCode()] + " has left the chat");
            Server.RemoveClient(listeningClientMessagesSocket.RemoteEndPoint.GetHashCode());
            listeningClientMessagesSocket.Close();
            listeningClientMessagesSocket = null;
        }

        public void NotifyClientLeft()
        {
            Server.SendToAll(new Message("has left the chat"));
            List<ClientsInfo> info = GetClientsList();
            Server.SendToAll(new Message(info));
        }

        public void DisconnectClient()
        {
            RemoveClient();
            NotifyClientLeft();
        }
    }
}
