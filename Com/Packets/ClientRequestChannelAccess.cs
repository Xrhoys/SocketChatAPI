using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ClientRequestChannelAccess: Packet, ICanSerialize
    {
        public int id;
        public int SenderID;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x0009);

            if(data.Length == 4)
            {
                id = BitConverter.ToInt32(data, 0);
                SenderID = BitConverter.ToInt32(data, 4);
            }
            else Debug.Fail("Unexpected data length.");
        }

        public byte[] Serialize()
        {
            using(var ms = new MemoryStream(8))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(id);
                bw.Write(SenderID);

                return ms.ToArray();
            }
        }
    }
}