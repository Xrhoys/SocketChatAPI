using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ServerAcknowledgementRequestChat: Packet, ICanSerialize
    {
        public int user1;
        public int user2;

        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x1003);

            if(data.Length > 0)
            {
                user1 = BitConverter.ToInt32(data, 0);
                user2 = BitConverter.ToInt32(data, 4);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            using(var ms = new MemoryStream(8))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(user1);
                bw.Write(user2);

                return ms.ToArray();
            }
        }
    }
}