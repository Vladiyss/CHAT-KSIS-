﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using ChatCommonInfo;

namespace Chat
{
    public class Client
    {
        const string BROADCASTIP = "192.168.1.255";
        const string LOCALADDRESS = "127.0.0.1";
        public const int SERVERPORT = 7744;
        Socket socketToCommunicateWithServer;
        Socket listeningUDPSocket;

        public bool isClientConnected = false;
        List<Thread> threadsList;

        MessageSerializer messageSerializer;

        public delegate void ReceivedMessagesHandler(Message message);
        public event ReceivedMessagesHandler ProcessReceivedMessagesEvent;

        public Client()
        {
            messageSerializer = new MessageSerializer();
            SetClientSocketForUDPListening();
            socketToCommunicateWithServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            threadsList = new List<Thread>();
        }

        public void SetClientSocketForUDPListening()
        {
            listeningUDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint IPendPoint = new IPEndPoint(IPAddress.Any, 0);
            listeningUDPSocket.Bind(IPendPoint);
        }

        public void UdpBroadcastRequest()
        {
            int clientPort = ((IPEndPoint)listeningUDPSocket.LocalEndPoint).Port;
            var message = new Message("192.168.1.102", clientPort, Message.MessageType[6]);

            IPAddress IPaddress = IPAddress.Parse(BROADCASTIP);
            IPEndPoint IPendPoint = new IPEndPoint(IPaddress, SERVERPORT);
            Socket sendConnectionRequestSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendConnectionRequestSocket.SendTo(messageSerializer.Serialize(message), IPendPoint);

            Thread threadReceiveUDPMessages = new Thread(ReceiveUDPMessages);
            threadsList.Add(threadReceiveUDPMessages);
            threadReceiveUDPMessages.Start();
        }

        public void ReceiveUDPMessages()
        {

            byte[] data = new byte[1024];
            EndPoint remotePoint = listeningUDPSocket.LocalEndPoint;
            while (true)
            {
                int amount = listeningUDPSocket.ReceiveFrom(data, ref remotePoint);
                Message receivedmessage = messageSerializer.Deserialize(data, amount);
                if (receivedmessage.messageType == Message.MessageType[7])
                {
                    OnMessageReceive(receivedmessage);
                    threadsList.Remove(Thread.CurrentThread);
                    CloseUDPSocket();
                    return;
                }
            }
        }

        public void ReceiveAllMessages()
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
                        amount = socketToCommunicateWithServer.Receive(data);
                        messageContainer.Write(data, 0, amount);
                    } while (socketToCommunicateWithServer.Available > 0);
                    Message recievedMessage = messageSerializer.Deserialize(messageContainer.GetBuffer(),
                        messageContainer.GetBuffer().Length);
                    OnMessageReceive(recievedMessage);
                }
                catch
                {
                    Disconnect();
                    return;
                }
            } while (isClientConnected);
        }

        public void OnMessageReceive(Message message)
        {
            ProcessReceivedMessagesEvent(message);
        }

        public bool ConnectToServer(IPEndPoint IPendPoint, string name)
        {
            if (Connect(IPendPoint))
            {
                SendMessage(new Message(name, Message.MessageType[4]));

                Thread communicationWithServerThread = new Thread(ReceiveAllMessages);
                threadsList.Add(communicationWithServerThread);
                communicationWithServerThread.Start();
                return true;
            }
            else
                return false;
        }

        public bool Connect(IPEndPoint endPoint)
        {
            try
            {
                socketToCommunicateWithServer.Connect(endPoint);
                isClientConnected = true;
                return true;
            }
            catch
            {
                Disconnect();
                return false;
            }
        }

        public bool SendMessage(Message message)
        {
            byte[] buffer = messageSerializer.Serialize(message);
            try
            {
                socketToCommunicateWithServer.Send(buffer);
            }
            catch
            {
                Disconnect();
            }
            return true;
        }

        public void CloseAllThreads()
        {
            foreach (Thread thread in threadsList)
            {
                thread.Abort();
            }
        }

        void CloseUDPSocket()
        {
            if (listeningUDPSocket != null)
            {
                listeningUDPSocket.Shutdown(SocketShutdown.Both);
                listeningUDPSocket.Close();
                listeningUDPSocket = null;
            }
        }

        void CloseMainSocket()
        {
            if (socketToCommunicateWithServer != null)
            {
                socketToCommunicateWithServer.Shutdown(SocketShutdown.Both);
                socketToCommunicateWithServer.Close();
                socketToCommunicateWithServer = null;
            }
        }

        public void Disconnect()
        {
            CloseMainSocket();
            isClientConnected = false;
        }

    }
}
