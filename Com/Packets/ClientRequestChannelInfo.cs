using System;
using System.Diagnostics;
using System.IO;

namespace Communication.Packets
{
    public class ClientRequestChannelInfo: Packet, ICanSerialize
    {
        public int id;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x0011);

            if(data.Length == 4)
            {
                id = BitConverter.ToInt32(data, 0);
            }
            else Debug.Fail("Unepexted packet length.");
        }

        public byte[] Serialize()
        {
            using(var ms = new MemoryStream(4))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(id);

                return ms.ToArray();
            }
        }
    }
}