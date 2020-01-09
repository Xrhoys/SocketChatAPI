namespace Communication.Packets
{
    public class ClientDisconnection: Packet, ICanSerialize
    {
        protected override void Deserialize(byte[] data) {}
        public byte[] Serialize() {
            return null;
        }
    }
}