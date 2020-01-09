using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Communication.Packets
{
    public class PacketSendMessage: Packet, ICanSerialize
    {
        public int userID;
        public int ChannelID;
        public string message;

        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x0001);

            if (data.Length > 0) {
                userID = BitConverter.ToInt32(data, 0);
                ChannelID = BitConverter.ToInt32(data, 4);
                message = Encoding.ASCII.GetString(data, 8, data.Length-8);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            using (var ms = new MemoryStream( messageBytes.Length + BitConverter.GetBytes(ChannelID).Length + 4))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(userID);
                bw.Write(ChannelID);
                bw.Write(messageBytes);

                return ms.ToArray();
            }
        }

        public override string ToString() => $"Message sent from {userID} on {ChannelID}: {message}";
    }
}