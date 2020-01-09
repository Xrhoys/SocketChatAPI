using System.Diagnostics;
using System.IO;
using System.Linq;
using Communication;
using System;

namespace Communication.Packets
{
    public abstract class Packet
    {
        public byte ID1;

        public byte ID2;

        public ushort ID => (ushort)(ID2 + (ID1 << 8));

        public byte[] Data;

        public static Packet Parse(byte[] allData)
        {
// #if DEBUG
//             var beforeDecrypt = allData.Clone();
// #endif

            //Parsing
            using (var ms = new MemoryStream(allData))
            using (var br = new BinaryReader(ms)) {
                var ID1 = br.ReadByte();
                var ID2 = br.ReadByte();
                byte[] packetData = br.ReadBytes(allData.Length-2);

                Packet pck = GetCorrectPacket((PacketType)(ID2 + (ID1 << 8)));
                pck.ID1 = ID1;
                pck.ID2 = ID2;
                pck.Data = packetData;
                pck.Deserialize(pck.Data);
                return pck;
            }
        }

        public static Packet GetCorrectPacket(PacketType t)
        {
            switch(t)
            {
                //List of packetTypes linked to packet classes
                case PacketType.ClientSendMessage: return new PacketSendMessage();
                case PacketType.ClientAccountLogin: return new ClientAccountLogin();
                case PacketType.ClientAccountLogout: return new ClientAccountLogout();
                case PacketType.ClientAccountRegister: return new ClientAccountRegister();
                case PacketType.ClientConnection: return new ClientConnection();
                case PacketType.ClientDisconnection: return new ClientDisconnection();
                case PacketType.ClientRequestChannelAccess: return new ClientRequestChannelAccess();
                case PacketType.ClientRequestChannelInfo: return new ClientRequestChannelInfo();
                case PacketType.ClientRequestChannelList: return new ClientRequestChannelList();
                case PacketType.ClientRequestChat: return new ClientRequestChat();
                case PacketType.ClientRequestCreateChannel: return new ClientRequestCreateChannel();
                case PacketType.ServerAcknowledgementChannelInfo: return new ServerAcknowledgementChannelInfo();
                case PacketType.ServerAcknowledgementConnection: return new ServerAcknowledgementConnection();
                case PacketType.ServerAcknowledgementCreateChannel: return new ServerAcknowledgementCreateChannel();
                case PacketType.ServerAcknowledgementDisconnection: return new ServerAcknowledgementDisconnection();
                case PacketType.ServerAcknowledgementLogin: return new ServerAcknowledgementLogin();
                case PacketType.ServerAcknowledgementLogout: return new ServerAcknowledgementLogout();
                case PacketType.ServerAcknowledgementRegister: return new ServerAcknowledgementRegister();
                case PacketType.ServerAcknowledgementRequestChannelAccess: return new ServerAcknowledgementRequestChannelAccess();
                case PacketType.ServerAcknowledgementRequestChannelList: return new ServerAcknowledgementRequestChannelList();
                case PacketType.ServerAcknowledgementRequestChat: return new ServerAcknowledgementRequestChat();
                default: return new GenericPacket();
            }
        }

        public static byte[] Encode(PacketType type, ICanSerialize p)
        {
            //serialize the packet
            byte[] data = p.Serialize();
            return Encode(type, data);
        }

        public static byte[] Encode(PacketType type, byte[] data)
        {
            short len = (short)(data.Length + 2);

            byte[] buffer = new byte[len];

            buffer[0] = type.ID1();
            buffer[1] = type.ID2();

            //fill in the data
            Array.Copy(data, 0, buffer, 2, data.Length);

            return buffer;
        }

        protected abstract void Deserialize(byte[] data);

        public override string ToString()
        {
            string dataString = Data.Length == 0 ? string.Empty : ": " + string.Join("-", Data.Select(x => x.ToString("X2")));
                return $"{this.IDString()}{dataString}"; 
        }
    }
}

public class GenericPacket: Communication.Packets.Packet, Communication.ICanSerialize
{
    protected override void Deserialize(byte[] data) { }

    public byte[] Serialize() => new byte[0];
}