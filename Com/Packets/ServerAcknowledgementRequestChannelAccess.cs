using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ServerAcknowledgementRequestChannelAccess: Packet, ICanSerialize
    {
        public int id;
        public bool success;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x1009);

            if(data.Length == 4)
            {
                id = BitConverter.ToInt32(data, 0);
                success = BitConverter.ToBoolean(data, 4);
            }
            else Debug.Fail("Unexpected data length.");
        }

        public byte[] Serialize()
        {
            using(var ms = new MemoryStream(4))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(id);
                bw.Write(success);

                return ms.ToArray();
            }
        }
    }
}