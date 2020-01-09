namespace Communication.Packets
{
    public class ClientConnection: Packet, ICanSerialize
    {
        protected override void Deserialize(byte[] data) {}
        public byte[] Serialize() {
            return null;
        }
    }
}