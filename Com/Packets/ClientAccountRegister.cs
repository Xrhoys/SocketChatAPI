using System.Text;
using System.IO;
using System.Diagnostics;
using System;

namespace Communication.Packets
{
    public class ClientAccountRegister: Packet, ICanSerialize
    {
        public string login;
        public string name;
        public string password;
        protected override void Deserialize(byte[] data)
        {
            Debug.Assert(ID == 0x0005);
            
            if(data.Length > 12)
            {
                int loginLen = BitConverter.ToInt32(data, 0);
                int nameLen = BitConverter.ToInt32(data, 4);
                int passwordLen = BitConverter.ToInt32(data, 8);
                login = Encoding.ASCII.GetString(data, 12,loginLen);
                name = Encoding.ASCII.GetString(data, 12 + loginLen, nameLen);
                password = Encoding.ASCII.GetString(data, 12 + loginLen + nameLen, passwordLen);
            }
            else Debug.Fail("Unexpected packet length.");
        }

        public byte[] Serialize()
        {
            byte[] loginBytes = Encoding.ASCII.GetBytes(login);
            byte[] nameBytes = Encoding.ASCII.GetBytes(name);
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);

            using(var ms = new MemoryStream())
            using(var bw = new BinaryWriter(ms)) {
                bw.Write(loginBytes.Length);
                bw.Write(nameBytes.Length);
                bw.Write(passwordBytes.Length);
                bw.Write(loginBytes);
                bw.Write(nameBytes);
                bw.Write(passwordBytes);
                
                return ms.ToArray();
            }
        }

        public override String ToString()
        {
            return $"{login}, {name}, {password}.";
        }
    }
}