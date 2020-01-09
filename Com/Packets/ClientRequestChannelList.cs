namespace Communication.Packets
{
    public class ClientRequestChannelList: Packet, ICanSerialize
    {
        protected override void Deserialize(byte[] data) { }

        public byte[] Serialize()
        {
            return null;
        }
    }
}