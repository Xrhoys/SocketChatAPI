using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ServerAcknowledgementCreateChannel: Packet, ICanSerialize
    {
        //I could just send the ID though ...
        public int id;
        public string name;
        public string description;
        protected override void Deserialize(byte[] data) 
        {
            Debug.Assert(ID == 0x1010);

            if(data.Length > 12)
            {
                id = BitConverter.ToInt32(data, 0);
                int nameLen = BitConverter.ToInt32(data, 4);
                int descriptionLen = BitConverter.ToInt32(data, 8);
                name = BitConverter.ToString(data, 12, 12 + nameLen);
                description = BitConverter.ToString(data, 12 + nameLen);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(name);
            byte[] descriptionBytes = Encoding.ASCII.GetBytes(description);

            using(var ms = new MemoryStream(8 + nameBytes.Length + descriptionBytes.Length))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(id);
                bw.Write(nameBytes.Length);
                bw.Write(descriptionBytes.Length);
                bw.Write(nameBytes);
                bw.Write(descriptionBytes);

                return ms.ToArray();
            }
        }
    }
}