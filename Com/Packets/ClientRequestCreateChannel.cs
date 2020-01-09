using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ClientRequestCreateChannel: Packet, ICanSerialize
    {
        public string name;
        public string description;
        protected override void Deserialize(byte[] data) 
        {
            Debug.Assert(ID == 0x0010);

            if(data.Length > 10)
            {
                int nameLen = BitConverter.ToInt32(data, 0);
                int descriptionLen = BitConverter.ToInt32(data, 4);
                name = BitConverter.ToString(data, 8, 8 + nameLen);
                description = BitConverter.ToString(data, 8 + nameLen);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = Encoding.ASCII.GetBytes(description);

            using(var ms = new MemoryStream(8 + nameBytes.Length + descriptionBytes.Length))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(nameBytes.Length);
                bw.Write(descriptionBytes.Length);
                bw.Write(nameBytes);
                bw.Write(descriptionBytes);

                return ms.ToArray();
            }
        }
    }
}