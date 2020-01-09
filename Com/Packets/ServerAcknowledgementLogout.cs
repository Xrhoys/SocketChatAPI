using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ServerAcknowledgementLogout: Packet, ICanSerialize
    {
        public bool success;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x1006);

            if(data.Length>0) 
            {
                success = BitConverter.ToBoolean(data, 0);
            }
            else Debug.Fail("Unexpect packet length.");
        }

        public byte[] Serialize()
        {
            using(var ms = new MemoryStream(1))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(success);

                return ms.ToArray();
            }
        }
    }
}