using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    class AllDialogsMessages
    {
        public Dictionary<int, string> FilesToSave = new Dictionary<int, string>();
        public List<string> Messages;
        public string Name;

        public AllDialogsMessages(string name)
        {
            Name = name;
            Messages = new List<string>();
        }
        public void AddMessage(string messageContent)
        {
            Messages.Add(messageContent);
        }
    }
}
