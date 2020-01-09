using System.Text;
using System.IO;
using System.Diagnostics;
using System;


namespace Communication.Packets
{
    public class ServerAcknowledgementRegister: Packet, ICanSerialize
    {
        public bool success;
        public string login;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x1005);
            if (data.Length > 1)
            {
                success = BitConverter.ToBoolean(data, 0);
                int loginLen = BitConverter.ToInt32(data, 1);
                login = Encoding.ASCII.GetString(data, 5, loginLen);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] loginBytes = Encoding.ASCII.GetBytes(login);
            using(var ms = new MemoryStream(5 + loginBytes.Length))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(success);
                bw.Write(loginBytes.Length);
                bw.Write(loginBytes);

                return ms.ToArray();
            }
        }

        public override String ToString()
        {
            return $"{login}";
        }
    }
}