using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Communication.Packets;

namespace Communication
{
    
    public class Net
    {
        public static void sendMsg(Stream s, Message msg)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(s, msg);
        }

        public static Message rcvMsg(Stream s)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (Message)bf.Deserialize(s);
        }

        public static void sendUser(Stream stream, User user)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, user);
        }

        public static User rcvUser(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (User)bf.Deserialize(stream);
        }

        public static int rcvConfirmUserId(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (int)bf.Deserialize(stream);
        }

        public static void sendConfirmUserId(Stream stream, int id)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, id);
        }

        //Sender and receiver for packets through simple net streams
        public static void sendPacket(Stream stream, Packet p)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(stream, p);
        }

        public static Packet receivePacket(Stream stream)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return (Packet)bf.Deserialize(stream);
        }
    }
}
