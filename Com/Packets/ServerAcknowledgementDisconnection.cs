namespace Communication.Packets
{
    public class ServerAcknowledgementDisconnection: Packet, ICanSerialize
    {
        protected override void Deserialize(byte[] data) {}
        public byte[] Serialize() {
            return null;
        }
    }
}