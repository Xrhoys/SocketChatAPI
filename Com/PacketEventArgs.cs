using System;
using Communication.Packets;
using System.Net.Sockets;

namespace Communication
{
    public class PacketEventArgs: EventArgs
    {
        public Socket Handler { get; }
        public Packet Packet { get; }
        public bool Outgoing { get; }

        public PacketEventArgs(Packet packet, bool outgoing, Socket handler)
        {
            Handler = handler;
            Packet = packet;
            Outgoing = outgoing;
        }
    }
}