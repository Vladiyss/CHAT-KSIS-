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
using ClientHTTP;
using System.IO;


namespace Chat
{
    public partial class MainForm : Form
    {
        const int CHATDIALOG = 0;
        Client client;
        ClientHTTP.Client httpClient;

        delegate void ProcessFormFilling();
        Dictionary<int, AllDialogsMessages> chatDialogsInfo;
        List<ClientsInfo> clientsInfo;
        int CurrentDialog = CHATDIALOG;
        int selectedReceiverIndex = 0;
        int selectedLoadedToServerFileIndex = -1;
        int selectedToSaveFileIndex = -1;

        public MainForm()
        {
            InitializeComponent();
            clientsInfo = new List<ClientsInfo>();
            clientsInfo.Add(new ClientsInfo() { clientID = CHATDIALOG, clientName = "Чат" });
            chatDialogsInfo = new Dictionary<int, AllDialogsMessages>();
            chatDialogsInfo.Add(CHATDIALOG, new AllDialogsMessages("Чат"));

            client = new Client();
            httpClient = new ClientHTTP.Client();
            client.ProcessReceivedMessagesEvent += ProcessReceivedMessages;
        }

        public void ProcessReceivedMessages(ChatCommonInfo.Message message)
        {
            int i = CommonInfo.RetrieveMessageType(message.messageType);
            switch (i)
            {
                case 1:
                    if (message.IPAdress == "")
                        chatDialogsInfo[CHATDIALOG].AddMessage(DateTime.Now.ToShortTimeString()
                        + " - " + message.messageName + " : " + message.messageContent);
                    else
                    chatDialogsInfo[CHATDIALOG].AddMessage(message.messageTime.ToString() + " - " + message.IPAdress
                        + " - " + message.messageName + " : " + message.messageContent);

                    if (message.areFilesSended)
                    {
                       foreach (ChatCommonInfo.FileInformaton fileInformaton in message.sendedFilesList)
                       {
                           chatDialogsInfo[CHATDIALOG].FilesToSave.Add(fileInformaton.fileID, fileInformaton.fileName);
                       }
                    }
                    break;
                case 2:
                    chatDialogsInfo[message.messageSenderID].AddMessage(message.messageTime.ToString() + " : " + message.messageContent);
                    labelNewMessage.Text = "Новое сообщение от " + message.messageName;

                    if (message.areFilesSended)
                    {
                        foreach (ChatCommonInfo.FileInformaton fileInformaton in message.sendedFilesList)
                        {
                            chatDialogsInfo[message.messageSenderID].FilesToSave.Add(fileInformaton.fileID, fileInformaton.fileName);
                        }
                    }
                    break;
                case 3:
                    chatDialogsInfo[CurrentDialog].Messages = message.messageHistory;
                    break;
                case 5:
                    {
                        ProcessFormFilling FormFillingNewClient = delegate
                        {
                            clientsInfo.Clear();
                            clientsInfo.Add(new ClientsInfo() { clientID = CHATDIALOG, clientName = "Чат" });
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
                            Invoke(FormFillingNewClient);
                        else
                            FormFillingNewClient();
                    }
                    break;
                case 7:
                    {
                        ProcessFormFilling FormFillingServerResponse = delegate
                        {
                            textBoxServerIPAdress.Text = message.IPAdress;
                            textBoxServerPort.Text = message.serverPort.ToString();
                            textBoxServerIPAdress.Enabled = false;
                            textBoxServerPort.Enabled = false;
                        };
                        if (InvokeRequired)
                            Invoke(FormFillingServerResponse);
                        else
                            FormFillingServerResponse();
                    }
                    break;
                default:
                    return;
            }
            if (i != 7)
                UpdateView();
        }

        public void UpdateView()
        {
            ProcessFormFilling FormUpdate = delegate
            {
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

                comboBoxAcceptedToSaveFiles.Items.Clear();
                foreach (KeyValuePair<int, string> filesToSave in chatDialogsInfo[CurrentDialog].FilesToSave)
                {
                    comboBoxAcceptedToSaveFiles.Items.Add(filesToSave);
                }

                labelCurrentClientDialog.Text = chatDialogsInfo[CurrentDialog].Name;
            };
            if (InvokeRequired)
                Invoke(FormUpdate);
            else
                FormUpdate();
        }

        private void buttonFindServer_Click(object sender, EventArgs e)
        {
            client.SetClientSocketForUDPListening();
            buttonConnectToServer.Enabled = true;
            buttonFindServer.Enabled = false;
        }

        private void buttonConnectToServer_Click(object sender, EventArgs e)
        {
            if (textBoxName.Text != "")
            {
                IPEndPoint IPendPoint = new IPEndPoint(IPAddress.Parse(textBoxServerIPAdress.Text), int.Parse(textBoxServerPort.Text));
                if (client.ConnectToServer(IPendPoint, textBoxName.Text))
                {
                    labelDisplayConnection.Text = "ПОДКЛЮЧЕНО";
                    buttonConnectToServer.Enabled = false;
                    buttonDisconnect.Enabled = true;
                    buttonSendMessage.Enabled = true;
                    buttonShowHistory.Enabled = true;
                }
                else
                    labelDisplayConnection.Text = "Нет соединения...";
            }
            else
                MessageBox.Show("Введите свой ник!","Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (client.isClientConnected)
            {
                client.Disconnect();
                buttonFindServer.Enabled = true;
                buttonDisconnect.Enabled = false;
                buttonSendMessage.Enabled = false;
                buttonShowHistory.Enabled = false;
                labelDisplayConnection.Text = "Нет соединения...";
                textBoxServerIPAdress.Enabled = true;
                textBoxServerIPAdress.Text = "";
                textBoxServerPort.Enabled = true;
                textBoxServerPort.Text = "";
                textBoxName.Enabled = true;
                textBoxName.Text = "";
                comboBoxParticipants.Items.Clear();
                richTextBoxChatContent.Clear();
                labelCurrentClientDialog.Text = "-";
                richTextBoxMessageContent.Clear();
                labelNewMessage.Text = "-";
            }   
            else
                labelDisplayConnection.Text = "...";
        }

        private void buttonSendMessage_Click(object sender, EventArgs e)
        {
            if (richTextBoxMessageContent.Text != "")
            {
                if (client.isClientConnected)
                {
                    string messagecontent = richTextBoxMessageContent.Text;
                    ChatCommonInfo.Message message;

                    if (CurrentDialog != CHATDIALOG)
                    {
                        message = new ChatCommonInfo.Message(CurrentDialog, messagecontent);
                        chatDialogsInfo[message.messageReceiverID].Messages.Add("Я : " + messagecontent);
                    }
                    else
                    {
                        message = new ChatCommonInfo.Message()
                        { messageContent = messagecontent, messageType = ChatCommonInfo.Message.MessageType[1], };
                    }

                    if (httpClient.LoadedToServerFiles.Count != 0)
                    {
                        labelFileManageStatus.Text = "OK";
                        message.areFilesSended = true;
                        var fileInformationList = new List<FileInformaton>();
                        foreach (KeyValuePair<int, string> keyValuePair in httpClient.LoadedToServerFiles)
                        {
                            fileInformationList.Add(new ChatCommonInfo.FileInformaton()
                            { fileID = keyValuePair.Key, fileName = keyValuePair.Value });
                        }

                        message.sendedFilesList = fileInformationList;
                        httpClient.LoadedToServerFiles.Clear();
                        httpClient.commonSizeOfLoadedFiles = 0;
                        selectedLoadedToServerFileIndex = -1;
                        UpdateFilesList();
                    }
                    else
                    {
                        message.areFilesSended = false;
                    }

                    client.SendMessage(message);
                    richTextBoxMessageContent.Clear();

                    UpdateView();
                }
            }
            else
                MessageBox.Show("Введите текст сообщения!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void buttonShowHistory_Click(object sender, EventArgs e)
        {
            comboBoxParticipants.SelectedIndex = 0;
            CurrentDialog = CHATDIALOG;
            selectedReceiverIndex = comboBoxParticipants.SelectedIndex;
            labelCurrentClientDialog.Text = chatDialogsInfo[CHATDIALOG].Name;
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

        void UpdateFilesList()
        {
            comboBoxDownloadedFiles.Items.Clear();
            foreach(KeyValuePair<int, string> file in httpClient.LoadedToServerFiles)
            {
                comboBoxDownloadedFiles.Items.Add(file);
            }
        }

        private async void buttonDownloadFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                string fileStatus = httpClient.CheckFileSizeAndExtension(filePath);
                if (String.IsNullOrEmpty(fileStatus))
                {
                    int fileID = await httpClient.LoadFileToServer(filePath);
                    labelFileManageStatus.Text = "Файл загружен";
                    httpClient.LoadedToServerFiles.Add(fileID, Path.GetFileName(filePath));
                    UpdateFilesList();
                }
                else
                    labelFileManageStatus.Text = fileStatus;
            }
        }

        private async void buttonDeleteDownloadedFile_Click(object sender, EventArgs e)
        {
            if (selectedLoadedToServerFileIndex != -1)
            {
                int fileID = ((KeyValuePair<int, string>)comboBoxDownloadedFiles.SelectedItem).Key;
                string deletingFileStatus = await httpClient.DeleteLoadedToServerFile(fileID);
                labelFileManageStatus.Text = deletingFileStatus;
                UpdateFilesList();
            }
            else
                labelFileManageStatus.Text = "Выберите файл для удаления!";
        }

        private void comboBoxDownloadedFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedLoadedToServerFileIndex = comboBoxDownloadedFiles.SelectedIndex;
        }

        private void comboBoxAcceptedToSaveFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedToSaveFileIndex = comboBoxAcceptedToSaveFiles.SelectedIndex;
        }

        private async void buttonGetFileInformation_Click(object sender, EventArgs e)
        {
            if (selectedToSaveFileIndex != -1)
            {
                int fileID = ((KeyValuePair<int, string>)comboBoxAcceptedToSaveFiles.SelectedItem).Key;
                string getFileInformationStatus = await httpClient.GetFileInformation(fileID);
                labelFileManageStatus.Text = getFileInformationStatus;
                labelFileName.Text = httpClient.requestedFileName;
                labelFileSize.Text = httpClient.requestedFileSize;
            }
            else
                labelFileManageStatus.Text = "Выберите файл для поиска информации!";
        }

        void SaveFileContent()
        {
            string fileName;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFileDialog1.FileName;
                FileStream outputFile = new FileStream(fileName, FileMode.Create);
                try
                {
                    outputFile.Write(httpClient.requestedFileContent, 0, httpClient.requestedFileContent.Length);
                }
                finally
                {
                    outputFile.Close();
                }
                Array.Clear(httpClient.requestedFileContent, 0, httpClient.requestedFileContent.Length);
            }
        }

        private async void buttonSaveFile_Click(object sender, EventArgs e)
        {
            if (selectedToSaveFileIndex != -1)
            {
                int fileID = ((KeyValuePair<int, string>)comboBoxAcceptedToSaveFiles.SelectedItem).Key;
                string saveFileStatus = await httpClient.GetFileToSave(fileID);
                if (String.IsNullOrEmpty(saveFileStatus))
                {
                    SaveFileContent();
                    labelFileManageStatus.Text = "Файл cохранён!";
                    chatDialogsInfo[CurrentDialog].FilesToSave.Remove(fileID);
                    UpdateView();
                }
                else
                    labelFileManageStatus.Text = saveFileStatus;
            }
            else
                labelFileManageStatus.Text = "Выберите файл для сохранения!";
        }
    }
}
