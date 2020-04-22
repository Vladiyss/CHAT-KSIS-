namespace Chat
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxServerIPAdress = new System.Windows.Forms.TextBox();
            this.textBoxServerPort = new System.Windows.Forms.TextBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.buttonFindServer = new System.Windows.Forms.Button();
            this.buttonConnectToServer = new System.Windows.Forms.Button();
            this.buttonDisconnect = new System.Windows.Forms.Button();
            this.richTextBoxChatContent = new System.Windows.Forms.RichTextBox();
            this.richTextBoxMessageContent = new System.Windows.Forms.RichTextBox();
            this.buttonSendMessage = new System.Windows.Forms.Button();
            this.labelDisplayConnection = new System.Windows.Forms.Label();
            this.buttonShowHistory = new System.Windows.Forms.Button();
            this.comboBoxParticipants = new System.Windows.Forms.ComboBox();
            this.labelCurrentClientDialog = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(25, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP-адрес сервера";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(25, 150);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "Порт сервера";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(25, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Ваше имя в чате";
            // 
            // textBoxServerIPAdress
            // 
            this.textBoxServerIPAdress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxServerIPAdress.Location = new System.Drawing.Point(190, 100);
            this.textBoxServerIPAdress.Name = "textBoxServerIPAdress";
            this.textBoxServerIPAdress.Size = new System.Drawing.Size(217, 24);
            this.textBoxServerIPAdress.TabIndex = 3;
            // 
            // textBoxServerPort
            // 
            this.textBoxServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxServerPort.Location = new System.Drawing.Point(190, 150);
            this.textBoxServerPort.Name = "textBoxServerPort";
            this.textBoxServerPort.Size = new System.Drawing.Size(217, 24);
            this.textBoxServerPort.TabIndex = 4;
            // 
            // textBoxName
            // 
            this.textBoxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxName.Location = new System.Drawing.Point(190, 200);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(217, 24);
            this.textBoxName.TabIndex = 5;
            // 
            // buttonFindServer
            // 
            this.buttonFindServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonFindServer.Location = new System.Drawing.Point(28, 25);
            this.buttonFindServer.Name = "buttonFindServer";
            this.buttonFindServer.Size = new System.Drawing.Size(379, 53);
            this.buttonFindServer.TabIndex = 6;
            this.buttonFindServer.Text = "Найти сервер";
            this.buttonFindServer.UseVisualStyleBackColor = true;
            this.buttonFindServer.Click += new System.EventHandler(this.buttonFindServer_Click);
            // 
            // buttonConnectToServer
            // 
            this.buttonConnectToServer.Enabled = false;
            this.buttonConnectToServer.Location = new System.Drawing.Point(28, 244);
            this.buttonConnectToServer.Name = "buttonConnectToServer";
            this.buttonConnectToServer.Size = new System.Drawing.Size(165, 32);
            this.buttonConnectToServer.TabIndex = 7;
            this.buttonConnectToServer.Text = "Подключиться";
            this.buttonConnectToServer.UseVisualStyleBackColor = true;
            this.buttonConnectToServer.Click += new System.EventHandler(this.buttonConnectToServer_Click);
            // 
            // buttonDisconnect
            // 
            this.buttonDisconnect.Enabled = false;
            this.buttonDisconnect.Location = new System.Drawing.Point(242, 244);
            this.buttonDisconnect.Name = "buttonDisconnect";
            this.buttonDisconnect.Size = new System.Drawing.Size(165, 32);
            this.buttonDisconnect.TabIndex = 8;
            this.buttonDisconnect.Text = "Отключиться";
            this.buttonDisconnect.UseVisualStyleBackColor = true;
            this.buttonDisconnect.Click += new System.EventHandler(this.buttonDisconnect_Click);
            // 
            // richTextBoxChatContent
            // 
            this.richTextBoxChatContent.Location = new System.Drawing.Point(459, 77);
            this.richTextBoxChatContent.Name = "richTextBoxChatContent";
            this.richTextBoxChatContent.Size = new System.Drawing.Size(590, 314);
            this.richTextBoxChatContent.TabIndex = 9;
            this.richTextBoxChatContent.Text = "";
            // 
            // richTextBoxMessageContent
            // 
            this.richTextBoxMessageContent.Location = new System.Drawing.Point(459, 410);
            this.richTextBoxMessageContent.Name = "richTextBoxMessageContent";
            this.richTextBoxMessageContent.Size = new System.Drawing.Size(434, 35);
            this.richTextBoxMessageContent.TabIndex = 10;
            this.richTextBoxMessageContent.Text = "";
            // 
            // buttonSendMessage
            // 
            this.buttonSendMessage.Enabled = false;
            this.buttonSendMessage.Location = new System.Drawing.Point(899, 410);
            this.buttonSendMessage.Name = "buttonSendMessage";
            this.buttonSendMessage.Size = new System.Drawing.Size(150, 35);
            this.buttonSendMessage.TabIndex = 11;
            this.buttonSendMessage.Text = "Отправить";
            this.buttonSendMessage.UseVisualStyleBackColor = true;
            this.buttonSendMessage.Click += new System.EventHandler(this.buttonSendMessage_Click);
            // 
            // labelDisplayConnection
            // 
            this.labelDisplayConnection.AutoSize = true;
            this.labelDisplayConnection.Location = new System.Drawing.Point(131, 298);
            this.labelDisplayConnection.Name = "labelDisplayConnection";
            this.labelDisplayConnection.Size = new System.Drawing.Size(14, 18);
            this.labelDisplayConnection.TabIndex = 13;
            this.labelDisplayConnection.Text = "-";
            // 
            // buttonShowHistory
            // 
            this.buttonShowHistory.Enabled = false;
            this.buttonShowHistory.Location = new System.Drawing.Point(151, 347);
            this.buttonShowHistory.Name = "buttonShowHistory";
            this.buttonShowHistory.Size = new System.Drawing.Size(256, 32);
            this.buttonShowHistory.TabIndex = 14;
            this.buttonShowHistory.Text = "Показать историю сообщений";
            this.buttonShowHistory.UseVisualStyleBackColor = true;
            this.buttonShowHistory.Click += new System.EventHandler(this.buttonShowHistory_Click);
            // 
            // comboBoxParticipants
            // 
            this.comboBoxParticipants.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxParticipants.FormattingEnabled = true;
            this.comboBoxParticipants.Location = new System.Drawing.Point(163, 410);
            this.comboBoxParticipants.Name = "comboBoxParticipants";
            this.comboBoxParticipants.Size = new System.Drawing.Size(244, 26);
            this.comboBoxParticipants.TabIndex = 15;
            this.comboBoxParticipants.SelectedIndexChanged += new System.EventHandler(this.comboBoxParticipants_SelectedIndexChanged);
            // 
            // labelCurrentClientDialog
            // 
            this.labelCurrentClientDialog.AutoSize = true;
            this.labelCurrentClientDialog.Location = new System.Drawing.Point(713, 25);
            this.labelCurrentClientDialog.Name = "labelCurrentClientDialog";
            this.labelCurrentClientDialog.Size = new System.Drawing.Size(14, 18);
            this.labelCurrentClientDialog.TabIndex = 16;
            this.labelCurrentClientDialog.Text = "-";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 410);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 18);
            this.label4.TabIndex = 17;
            this.label4.Text = "Участники";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1091, 588);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelCurrentClientDialog);
            this.Controls.Add(this.comboBoxParticipants);
            this.Controls.Add(this.buttonShowHistory);
            this.Controls.Add(this.labelDisplayConnection);
            this.Controls.Add(this.buttonSendMessage);
            this.Controls.Add(this.richTextBoxMessageContent);
            this.Controls.Add(this.richTextBoxChatContent);
            this.Controls.Add(this.buttonDisconnect);
            this.Controls.Add(this.buttonConnectToServer);
            this.Controls.Add(this.buttonFindServer);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.textBoxServerPort);
            this.Controls.Add(this.textBoxServerIPAdress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "MainForm";
            this.Text = "Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxServerIPAdress;
        private System.Windows.Forms.TextBox textBoxServerPort;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonFindServer;
        private System.Windows.Forms.Button buttonConnectToServer;
        private System.Windows.Forms.Button buttonDisconnect;
        private System.Windows.Forms.RichTextBox richTextBoxChatContent;
        private System.Windows.Forms.RichTextBox richTextBoxMessageContent;
        private System.Windows.Forms.Button buttonSendMessage;
        private System.Windows.Forms.Label labelDisplayConnection;
        private System.Windows.Forms.Button buttonShowHistory;
        private System.Windows.Forms.ComboBox comboBoxParticipants;
        private System.Windows.Forms.Label labelCurrentClientDialog;
        private System.Windows.Forms.Label label4;
    }
}

