using System;
using System.Collections;

namespace Communication
{
    [Serializable]
    public class Channel
    {
        public static ArrayList _channel_list = new ArrayList();

        private ArrayList _connected_user;

        private static int _id_increment = 0;
        private int _id;
        public int id
        {
            get { return _id; }
        }

        private string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        private ArrayList _messageList;

        public Channel(string name, string description)
        {
            _id = ++_id_increment;
            _name = name;
            _description = description;
            _messageList = new ArrayList();
            _connected_user = new ArrayList();
            _channel_list.Add(this);
        }

        public Channel() {}
        public void Add(Message message)
        {
            this._messageList.Add(message);
        }

        public void Delete(Message message)
        {
            if (_messageList.Contains(message))
            {
                _messageList.Remove(message);
            }
        }

        public void AddUser(int user)
        {
            if (user != 0)
            {
                _connected_user.Add(user);
            }
        }

        public void RemoveUser(int user)
        {
            if (_connected_user.Contains(user))
            {
                _connected_user.Remove(user);
            }
        }

        public bool ContainsUser(int user)
        {
            foreach(User ur in _connected_user)
            {
                if(ur.id == user)
                {
                    return true;
                }
            }
            return false;
        }

        public ArrayList GetUserList()
        {
            return _connected_user;
        }

        public ArrayList GetMessageList()
        {
            return _messageList;
        }

        public static Channel GetChannelById(int id)
        {
            foreach (Channel ch in _channel_list)
            {
                if (ch._id == id)
                {
                    return ch;
                }
            }
            Console.WriteLine("Channel not found.");
            return null;
        }

        public static Channel GetChannelByName(string name)
        {
            foreach (Channel ch in _channel_list)
            {
                if (ch._name == name)
                {
                    return ch;
                }
            }
            return null;
        }

        public static Channel CreateNew(string name, string description)
        {
            //TODO: dont forget to change the constructor!
            Channel channel = new Channel();

            channel._id = ++_id_increment;
            channel._name = name;
            channel._description = description;
            channel._messageList = new ArrayList();
            channel._connected_user = new ArrayList();
            _channel_list.Add(channel);

            return channel;
        }

        protected static void Delete(int id)
        {
            foreach(Channel ch in _channel_list)
            {
                if(ch._id == id)
                {
                    _channel_list.Remove(ch);
                    return;
                }
            }
            Console.WriteLine("Error, channel id doesn't exist.");
        }

        //ClientSide
        public static Channel Attribute(int id, string name, string description)
        {
            Channel channel = new Channel();

            channel._id = id;
            channel.name = name;
            channel.description = description;
            channel._messageList = new ArrayList();
            channel._connected_user = new ArrayList();
            _channel_list.Add(channel);

            return channel;
        }
    }

    public class Chat : Channel
    {
        private ArrayList message_log;

        private static ArrayList chat_list = new ArrayList();
        //Pair reference to store user ID
        private Tuple<int, int> users;
        public Chat(string name, string description, int user1, int user2) : base(name, description)
        {
            this.message_log = new ArrayList();
        }

        public Chat( ) { }

        public static Chat CreateNew(int user1, int user2)
        {
            //check if it exists
            Chat chat = IfExists(user1, user2);
            if(chat != null)
            {
                return chat;
            }

            //TODO: code cleaning for the constructor
            chat.message_log = new ArrayList();
            chat.users = new Tuple<int, int>(user1, user2);
            chat_list.Add(chat.users);

            return chat;
        }

        private static Chat IfExists(int user1, int user2)
        {
            foreach(Chat t in chat_list)
            {
                if(t.users.Item1 == user1 && t.users.Item2 == user2)
                {
                    return t;
                }
            }
            return null;
        }

        private static void Remove(int user1, int user2)
        {
            Chat chat = IfExists(user1, user2);
            if(chat != null)
            {
                chat_list.Remove(chat);
            }
            else
            {
                Console.WriteLine("Error, non recognized id.");
            }
        }

        public Tuple<int, int> getId()
        {
            return users;
        }
    }
}
