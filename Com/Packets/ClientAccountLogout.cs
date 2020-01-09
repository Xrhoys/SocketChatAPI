using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ClientAccountLogout: Packet, ICanSerialize 
    {
        public int id;
        protected override void Deserialize(byte[] data) 
        {
            if(data.Length != 4)
            {
                id = BitConverter.ToInt32(data, 0);
            }
            else Debug.Fail("Unexpected packet length.");
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