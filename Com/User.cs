using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;

namespace Communication
{
    [Serializable]
    public class User
    {
        private static Dictionary<String, User> _user_list = new Dictionary<string, User>();
        private static int _id_increment = 0;

        private Dictionary<User, Chat>  _private_chat ;

        public int socketID = 0;

        private int _id;
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _username;
        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _password;
        public string password
        {
            get { return _password; }
            set { _password = value; }
        }

        private string _email;
        public string email
        {
            get { return _email; }
            set { _email = value; }
        }

        public User(string name, string username, string email)
        {
            _id_increment++;
            _id = _id_increment;
            _name = name;
            _username = username;
            _email = email;
            try
            {
                _user_list.Add(this._name, this);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.ToString());
            }
            _private_chat = new Dictionary<User, Chat>();
        }

        public User() {}

        public static bool Register(string login, string password)
        {
            User user = new User();
            user.login = login;
            user.password = password;
            _id_increment++;
            user._id = _id_increment;
            try
            {
                _user_list.Add(user.login, user);
                Console.WriteLine("User added.");
                return true;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        private string login;

        public static bool validateUser(string username, string password)
        {
            if (_user_list.ContainsKey(username))
            {
                if (_user_list[username].password.Equals(password))
                {
                    Console.WriteLine("Validated " + username + ".");
                    return true;
                }
                else
                {
                    Console.WriteLine("Incorrect Credentials.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Unknown user.");
                return false;
            }
        }

        // public Chat requestChat(User request_user){
        //     if(_private_chat.ContainsKey(request_user))
        //     {
        //         return _private_chat[request_user];
        //     }
        //     else
        //     {
        //         _private_chat.Add(request_user, new Chat("private", "private chat"));
        //         return _private_chat[request_user];
        //     }
        // }

        public static Dictionary<String, User> getUserList()
        {
            return _user_list;
        }

        public static User GetUserByID(int id)
        {
            foreach(User x in _user_list.Values)
            {
                if(x._id == id)
                {
                    return x;
                }
            }
            return null;
        }

        public void connectToCurrentClient(int socketID)
        {
            this.socketID = socketID;
        }

        public void unbindSocketID()
        {
            this.socketID = 0;
        }

        ~User()
        {
            if (_user_list.ContainsKey(this._name))
            {
                _user_list.Remove(this._name);
                _private_chat.Clear();
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }
    }
}