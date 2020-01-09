using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ClientAccountLogin: Packet, ICanSerialize
    {
        public string login;
        public string password;

        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x0004);

            if(data.Length > 0)
            {
                int loginLen = BitConverter.ToInt32(data, 0);
                int passwordLen = BitConverter.ToInt32(data, 4);
                login = Encoding.ASCII.GetString(data, 8, loginLen);
                password = Encoding.ASCII.GetString(data, 8 + loginLen, passwordLen);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] loginBytes = Encoding.ASCII.GetBytes(login);
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);

            using(var ms = new MemoryStream(loginBytes.Length + passwordBytes.Length + 8))
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(loginBytes.Length);
                bw.Write(passwordBytes.Length);
                bw.Write(loginBytes);
                bw.Write(passwordBytes);

                return ms.ToArray();
            }
        }
    }
}