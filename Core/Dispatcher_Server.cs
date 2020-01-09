using System;
using Communication;
using Communication.Packets;
using System.Collections;

namespace Core
{
    public class Dispatcher_Server
    {
        public static ArrayList SendingQueue = new ArrayList();
        public static Packet loginPacket;
        public Dispatcher_Server() { }

        public static byte[] Switcher(Packet p)
        {
            PacketType type;
            Packet packet;
            switch(p.ID)
            {
                case (ushort)PacketType.ClientSendMessage: 
                    type = PacketType.ClientSendMessage;
                    packet = AcceptMessage(p);
                    break;
                case (ushort)PacketType.ClientRequestChannelList: 
                    type = PacketType.ServerAcknowledgementRequestChannelList;
                    packet = ClientRequestChannelList(p);
                    break;
                case (ushort)PacketType.ClientRequestChat: 
                    type = PacketType.ServerAcknowledgementRequestChat;
                    packet = CreateChat(p);
                    break;
                case (ushort)PacketType.ClientAccountLogin: 
                    type = PacketType.ServerAcknowledgementLogin;
                    packet = UserAuthentification(p);
                    break;
                case (ushort)PacketType.ClientAccountRegister: 
                    type = PacketType.ServerAcknowledgementRegister;
                    packet = UserRegistration(p);
                    break;
                case (ushort)PacketType.ClientAccountLogout: 
                    type = PacketType.ServerAcknowledgementLogout;
                    packet = UserLogout(p);
                    break;
                case (ushort)PacketType.ClientConnection: 
                    type = PacketType.ServerAcknowledgementConnection;
                    packet = ClientConnection(p);
                    break;
                case (ushort)PacketType.ClientDisconnection: 
                    type = PacketType.ServerAcknowledgementDisconnection;
                    packet = ClientDisconnection(p);
                    break;
                case (ushort)PacketType.ClientRequestChannelAccess: 
                    type = PacketType.ServerAcknowledgementRequestChannelAccess;
                    packet = RequestChannelAccess(p);
                    break;
                case (ushort)PacketType.ClientRequestCreateChannel: 
                    type = PacketType.ServerAcknowledgementCreateChannel;
                    packet = CreateChannel(p);
                    break;
                case (ushort)PacketType.ClientRequestChannelInfo: 
                    type = PacketType.ServerAcknowledgementChannelInfo;
                    packet = RequestChannelInfo(p);
                    break;
                default: 
                    type = (ushort)0;
                    packet = new GenericPacket();
                    break;
            }

            //Format the answer
            Console.WriteLine(type.ID());
            if(packet != null && type != 0)
            {
                return Packet.Encode(type, (ICanSerialize)packet);
            }
            else
            {
                return new byte[0];
            }
        }

        private static Packet AcceptMessage(Packet p)
        {
            try
            {
                PacketSendMessage packet = (PacketSendMessage)p;
                Channel channel = Channel.GetChannelById(packet.ChannelID);
                if (channel != null && channel.ContainsUser(packet.userID))
                {
                    User_Message message =
                        new User_Message(packet.userID, packet.message, packet.ChannelID);
                    channel.Add(message);
                    Console.WriteLine(packet.ToString());
                    Broadcast(channel, message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet CreateChannel(Packet p)
        {
            try
            {
                ClientRequestCreateChannel packet = (ClientRequestCreateChannel)p;
                Console.WriteLine("Received request to create new channel.");
                //Create channel
                Channel channel = Channel.CreateNew(packet.name, packet.description);

                //Format response
                ServerAcknowledgementCreateChannel response = new ServerAcknowledgementCreateChannel();
                response.id = channel.id;
                response.name = channel.name;
                response.description = channel.description;
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet CreateChat(Packet p)
        {
            try
            {
                ClientRequestChat packet = (ClientRequestChat)p;
                //Check user existence
                if(User.GetUserByID(packet.user1) != null || User.GetUserByID(packet.user2) != null)
                {
                    ServerAcknowledgementRequestChat response = new ServerAcknowledgementRequestChat();
                    response.user1 = 0;
                    response.user2 = 0;
                    return response;
                }
                if(packet.user1 > 0 && packet.user2 > 0){
                    Chat chat = Chat.CreateNew(packet.user1, packet.user2);
                    ServerAcknowledgementRequestChat response = new ServerAcknowledgementRequestChat();
                    response.user1 = chat.getId().Item1;
                    response.user2 = chat.getId().Item2;
                    return response;
                }
                else
                {
                    return new GenericPacket();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet ClientRequestChannelList(Packet p)
        {
            try
            {
                ClientRequestChannelList packet = (ClientRequestChannelList)p;
                ServerAcknowledgementRequestChannelList response = new ServerAcknowledgementRequestChannelList();
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet UserAuthentification(Packet p)
        {
            try
            {
                ClientAccountLogin packet = (ClientAccountLogin)p;
                ServerAcknowledgementLogin response = new ServerAcknowledgementLogin();
                //Authentification process: modify IsLoggedIn and login variable of the response packet
                if (User.validateUser(packet.login, packet.password))
                {
                    Console.WriteLine("User connected. Sending response.");
                    response.verification = true;
                    response.login = packet.login;
                }
                else
                {
                    response.verification = false;
                    response.login = "none";
                }
                loginPacket = response;
                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet UserRegistration(Packet p)
        {
            try
            {
                ClientAccountRegister packet = (ClientAccountRegister)p;
                Console.WriteLine(packet.ToString());
                ServerAcknowledgementRegister response = new ServerAcknowledgementRegister();
                response.login = packet.login;
                //Registration attempt
                if(User.Register(packet.login, packet.password))
                {
                    response.success = true;
                    return response;
                }
                else
                {
                    response.success = false;
                    return response;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet UserLogout(Packet p)
        {
            try
            {  
               ClientAccountLogout packet = (ClientAccountLogout)p;
               //Process logout procedure
               User.GetUserByID(packet.id)?.unbindSocketID();
               ServerAcknowledgementLogout response = new ServerAcknowledgementLogout();
               response.success = true;
               return response; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet ClientConnection(Packet p)
        {
            return new ServerAcknowledgementConnection();
        }

        private static Packet ClientDisconnection(Packet p)
        {
            return new ServerAcknowledgementDisconnection();
        }

        private static Packet RequestChannelAccess(Packet p)
        {
            try
            {
                ClientRequestChannelAccess packet = new ClientRequestChannelAccess();
                Channel channel = Channel.GetChannelById(packet.id);
                channel.AddUser(packet.SenderID);

                ServerAcknowledgementRequestChannelAccess response = new ServerAcknowledgementRequestChannelAccess();
                response.success = true;
                response.id = channel.id;

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static Packet RequestChannelInfo(Packet p)
        {
            try
            {
                ClientRequestChannelInfo packet = (ClientRequestChannelInfo)p;
                Channel channel = Channel.GetChannelById(packet.id);

                ServerAcknowledgementChannelInfo response = new ServerAcknowledgementChannelInfo();
                response.id = channel.id;
                response.name = channel.name;
                response.description = channel.description;

                return response;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        private static void Broadcast(Channel channel, Message message)
        {
            foreach(int userID in channel.GetUserList())
            {
                User user = User.GetUserByID(userID);
                if(user.socketID == 0)
                {
                    continue;
                }
                PacketSendMessage packet = new PacketSendMessage();
                packet.userID = user.id;
                packet.message = message.GetContent();
                packet.ChannelID = channel.id;
                SendingQueue.Add(packet);
            }
        }
    }
}