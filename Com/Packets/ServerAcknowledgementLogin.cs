using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ServerAcknowledgementLogin: Packet, ICanSerialize
    {
        public bool verification;
        public string login;
        public int id;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x1004);

            if(data.Length > 0)
            {
                verification = BitConverter.ToBoolean(data, 0);
                id = BitConverter.ToInt32(data, 1);
                login = Encoding.ASCII.GetString(data, 5, data.Length-5);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] loginBytes = Encoding.ASCII.GetBytes(login);
            using(var ms = new MemoryStream(1 + loginBytes.Length + 4))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(verification);
                bw.Write(id);
                bw.Write(login);

                return ms.ToArray();
            }
        }

        public override string ToString()
        {
            return $"{id}: {login}";
        }
    }
}