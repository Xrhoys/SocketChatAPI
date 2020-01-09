namespace Communication.Packets
{
    public class ServerAcknowledgementConnection: Packet, ICanSerialize
    {
        protected override void Deserialize(byte[] data) {}
        public byte[] Serialize() {
            return null;
        }
    }
}