using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using ChatCommonInfo;


namespace Chat
{
    public partial class MainForm : Form
    {
        const int CHATDIALOG = 0;
        Client client;

        Dictionary<int, AllDialogsMessages> chatDialogsInfo;
        List<ClientsInfo> clientsInfo;
        int CurrentDialog = CHATDIALOG;
        int selectedReceiverIndex = 0;

        public MainForm()
        {
            InitializeComponent();
            clientsInfo = new List<ClientsInfo>();
            clientsInfo.Add(new ClientsInfo() { clientID = CHATDIALOG, clientName = "Chat" });
            chatDialogsInfo = new Dictionary<int, AllDialogsMessages>();
            chatDialogsInfo.Add(CHATDIALOG, new AllDialogsMessages("Chat"));

            client = new Client();
            client.ProcessReceivedMessagesEvent += ProcessReceivedMessages;
        }

        int RetrieveMessageType(string currentType)
        {
            int i = 1;
            bool flag = true;
            while ((i <= 7) && flag)
            {
                int result = String.Compare(ChatCommonInfo.Message.MessageType[i], currentType);
                if (result == 0)
                    flag = false;
                else
                    ++i;
            }
            return i;
        }

        public void ProcessReceivedMessages(ChatCommonInfo.Message message)
        {
            int i = RetrieveMessageType(message.messageType);
            switch (i)
            {
                case 1:
                    chatDialogsInfo[CHATDIALOG].AddMessage(message.messageName + " : "
                        + message.messageContent + "\n" + message.messageTime.ToString() + "\n"
                        + message.IPAdress + message.clientPort.ToString());
                    break;
                case 2:
                    chatDialogsInfo[message.messageSenderID].AddMessage(message.messageContent + "\n" +
                        message.messageTime + "\n" + message.IPAdress + message.clientPort.ToString());
                    break;
                case 3:
                    chatDialogsInfo[message.messageReceiverID].Messages = message.messageHistory;
                    break;
                case 5:
                    {
                        Action action = delegate
                        {
                            clientsInfo.Clear();
                            clientsInfo.Add(new ClientsInfo() { clientID = CHATDIALOG, clientName = "Chat" });
                            foreach (ClientsInfo nameClient in message.clientsInfo)
                            {
                                clientsInfo.Add(nameClient);
                                if (!chatDialogsInfo.ContainsKey(nameClient.clientID))
                                {
                                    chatDialogsInfo.Add(nameClient.clientID, new AllDialogsMessages(nameClient.clientName));
                                }
                            }
                        };
                        if (InvokeRequired)
                            Invoke(action);
                        else
                            action();
                    }
                    break;
                case 7:
                    {
                        Action action = delegate
                        {
                            textBoxServerIPAdress.Text = message.IPAdress;
                            textBoxServerPort.Text = message.serverPort.ToString();
                            textBoxServerIPAdress.Enabled = false;
                            textBoxServerPort.Enabled = false;
                        };
                        if (InvokeRequired)
                            Invoke(action);
                        else
                            action();
                    }
                    break;
                default:
                    return;
            }
            UpdateView();
        }

        public void UpdateView()
        {
            Action action = delegate
            {
                /*if (CurrentDialog == CHATDIALOG)
                {
                    comboBoxParticipants.Text = chatDialogsInfo[CurrentDialog].Name;
                }
                else
                {
                    comboBoxParticipants.Text = comboBoxParticipants.SelectedItem.ToString();
                } */
                richTextBoxChatContent.Clear();
                if (chatDialogsInfo != null)
                {
                    string[] messages = new string[chatDialogsInfo[CurrentDialog].Messages.Count];
                    chatDialogsInfo[CurrentDialog].Messages.CopyTo(messages);
                    foreach (string messageContent in messages)
                    {
                        richTextBoxChatContent.Text += messageContent + "\r\n";
                    }
                }
                comboBoxParticipants.Items.Clear();
                foreach (ClientsInfo clientInfo in clientsInfo)
                {
                    comboBoxParticipants.Items.Add(clientInfo.clientName);
                }

                labelCurrentClientDialog.Text = chatDialogsInfo[CurrentDialog].Name;
            };
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void buttonFindServer_Click(object sender, EventArgs e)
        {
            client.UdpBroadcastRequest();
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            IPEndPoint IPendPoint = new IPEndPoint(IPAddress.Parse(textBoxServerIPAdress.Text), int.Parse(textBoxServerPort.Text));
            if (client.ConnectToServer(IPendPoint, textBoxName.Text))
                labelDisplayConnection.Text = "Подключено";
            else
                labelDisplayConnection.Text = "Нет соединения...";
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (client.isClientConnected)
            {
                client.Disconnect();
                labelDisplayConnection.Text = "Нет соединения...";
                client.isClientConnected = false;
            }   
            else
                labelDisplayConnection.Text = "...";
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if (selectedReceiverIndex != -1)
            {
                if (client.isClientConnected)
                {
                    string messagecontent = richTextBoxMessageContent.Text;
                    ChatCommonInfo.Message message;
                    if (CurrentDialog != CHATDIALOG)
                    {
                        message = new ChatCommonInfo.Message(CurrentDialog, messagecontent);
                        chatDialogsInfo[message.messageReceiverID].Messages.Add("Me : " + messagecontent);
                    }
                    else
                    {
                        message = new ChatCommonInfo.Message()
                        { messageName = "Me", messageContent = messagecontent, messageType = ChatCommonInfo.Message.MessageType[1] };
                    }
                    client.SendMessage(message);

                    UpdateView();
                }
            }
        }

        private void buttonShowHistory_Click(object sender, EventArgs e)
        {
            comboBoxParticipants.SelectedIndex = 0;
            selectedReceiverIndex = comboBoxParticipants.SelectedIndex;
            var message = new ChatCommonInfo.Message() { messageType = ChatCommonInfo.Message.MessageType[3] };
            client.SendMessage(message);
            UpdateView();
        }

        private void comboBoxParticipants_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxParticipants.SelectedIndex != -1)
            {
                selectedReceiverIndex = comboBoxParticipants.SelectedIndex;
                CurrentDialog = clientsInfo[selectedReceiverIndex].clientID;
                UpdateView();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            client.CloseAllThreads();
        }
    }
}
