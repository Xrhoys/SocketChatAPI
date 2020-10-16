using System;
using Communication;
using Communication.Packets;

namespace Core
{
    public class Dispatcher_Client
    {
        public Dispatcher_Client() {  }

        public static byte[] Switcher(Packet p)
        {
            PacketType type = (ushort)0;
            Packet packet;
            switch(p.ID)
            {
                case (ushort)PacketType.ServerAcknowledgementChannelInfo: 
                    packet = ReceiveChannelInfo(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementConnection: 
                    packet = Connection(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementCreateChannel: 
                    packet = CreateChannel(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementDisconnection: 
                    packet = Disconnection(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementLogin: 
                    packet = Login(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementLogout: 
                    packet = Logout(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementRegister: 
                    packet = Register(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementRequestChannelAccess: 
                    packet = RequestChannelAccess(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementRequestChannelList: 
                    packet = RequestChannelList(p);
                    break;
                case (ushort)PacketType.ServerAcknowledgementRequestChat: 
                    packet = RequestChat(p);
                    break;
                case (ushort)PacketType.ClientSendMessage: 
                    packet = SendMessage(p);
                    break;
                default: packet = new GenericPacket(); break;
            }
            if(packet != null && type != 0)
            {
                return Packet.Encode(type, (ICanSerialize)packet);
            }
            else
            {
                return new byte[0];
            }
        }

        private static Packet ReceiveChannelInfo(Packet p)
        {
            try
            {
                ServerAcknowledgementChannelInfo packet = (ServerAcknowledgementChannelInfo)p;
                Channel channel = Channel.GetChannelById(packet.id);

                channel.name = packet.name;
                channel.description = packet.description;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet Connection(Packet p)
        {
            ServerAcknowledgementConnection packet = (ServerAcknowledgementConnection)p;
            //Connection parameters? Put presets here
            return null;
        }

        private static Packet CreateChannel(Packet p)
        {
            ServerAcknowledgementCreateChannel packet = (ServerAcknowledgementCreateChannel)p;
            Channel channel = Channel.Attribute(packet.id, packet.name, packet.description);

            return null;
        }

        private static Packet Disconnection(Packet p)
        {
            ServerAcknowledgementDisconnection packet = (ServerAcknowledgementDisconnection)p;
            //Disconnection steps
            return null;
        }

        private static Packet Login(Packet p)
        {
            ServerAcknowledgementLogin packet = (ServerAcknowledgementLogin)p;
            if (packet.verification)
            {   
                //Login logic (create static user) + notification
                User user = new User();
                user.id = packet.id;
                user.username = packet.login;
                Session.currentUser = user;
                Session.isLoggedIn = true;
                Console.WriteLine("Connected as {0}.", packet.login);
            }
            return null;
        }

        private static Packet Logout(Packet p)
        {
            ServerAcknowledgementLogout packet = (ServerAcknowledgementLogout)p;
            if (packet.success)
            {
                //Logout logic (remove static user) + notification
                Session.currentUser = null;
                Session.isLoggedIn = false;
            }
            return null;
        }

        private static Packet Register(Packet p)
        {
            ServerAcknowledgementRegister packet = (ServerAcknowledgementRegister)p;
            if (packet.success)
            {
                //Register logic + notification
                Console.WriteLine("User registered.");
            }
            return null;
        }

        private static Packet RequestChannelAccess(Packet p)
        {
            ServerAcknowledgementRequestChannelAccess packet = (ServerAcknowledgementRequestChannelAccess)p;
            if (packet.success)
            {
                //Channel access granted, notify the user
                Console.WriteLine("Acces granted to channel {0}", packet.id);
            }
            return null;
        }

        private static Packet RequestChannelList(Packet p)
        {
            ServerAcknowledgementRequestChannelList packet = (ServerAcknowledgementRequestChannelList)p;
            foreach(int ch in packet.list)
            {
                Channel channel = Channel.Attribute(ch, null, null);
                Console.WriteLine(channel.id);
            }
            return null;
        }

        private static Packet RequestChat(Packet p)
        {
            ServerAcknowledgementRequestChat packet = (ServerAcknowledgementRequestChat)p;

            if(packet.user1 != 0 && packet.user2 != 0)
            {
                Chat chat = Chat.CreateNew(packet.user1, packet.user2);
                //Additional sending
            }
            return null;
        }

        private static Packet SendMessage(Packet p)
        {
            PacketSendMessage packet = (PacketSendMessage)p;
            Console.WriteLine(packet.ToString());

            return null;
        }
    }
}