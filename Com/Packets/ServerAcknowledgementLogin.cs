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
                int loginLen = BitConverter.ToInt32(data, 5);
                login = Encoding.ASCII.GetString(data, 9, loginLen);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] loginBytes = Encoding.ASCII.GetBytes(login);
            using(var ms = new MemoryStream(9 + loginBytes.Length))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(verification);
                bw.Write(id);
                bw.Write(loginBytes.Length);
                bw.Write(loginBytes);

                return ms.ToArray();
            }
        }

        public override string ToString()
        {
            return $"{id}: {login}";
        }
    }
}