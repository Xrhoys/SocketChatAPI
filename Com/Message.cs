using System;
using System.IO;
using System.Text;

namespace Communication
{

    public interface Message
    {
        string ToString();
        string GetContent();
        void SetTargetID(int id);
    }

    [Serializable]
    public class User_Message: Message
    {
        private int id;
        private string content;
        private DateTime date;

        public int channel_id = 0;

        public int targetID = 0;
        public User_Message(int user, string content, int channel_id){
            this.id = user;
            this.content = content;
            this.date = DateTime.Now;
            this.channel_id = channel_id;
        }

        public void SetTargetID(int id)
        {
            this.targetID = id;
        }

        public string GetContent()
        {
            return content;
        }

        public override string ToString(){
            return "From channel " + channel_id + ":\n Sender: " + id + "\n" 
                    + date.ToString("F") + " :\n" + content;
        }
    }
}
