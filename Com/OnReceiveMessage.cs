using Communication.Packets;

namespace Communication
{
    public class OnReceiveMessage
    {
        public int SenderID {get;}
        public Message packet {get;}

        public OnReceiveMessage(int SenderID, Message packet)
        {
            this.SenderID = SenderID;
            this.packet = packet;
        }
    }
}