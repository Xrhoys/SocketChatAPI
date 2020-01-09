using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using Communication;
using System.Collections.Generic;
using System.Collections;

namespace Server
{
    public class AsyncSocketServer
    {
        private int port;

        private int loginPort;

        public static Channel main;

        public static ArrayList privateChatList = new ArrayList();

        private static readonly object _lock = new object();
        private static readonly Dictionary<int, TcpClient> list_clients 
            = new Dictionary<int, TcpClient>();

        public AsyncSocketServer(int port, int loginPort)
        {
            this.port = port;
            this.loginPort = loginPort;
            if(main == null)
            {
                main = new Channel("main", "main dev test channel");
                Console.WriteLine("Server initialization ...");
                Console.WriteLine("Main test channel created.");
                Console.WriteLine(main.id);
            }
        }

        public void Start()
        {
            int count = 1;

            try
            {
                TcpListener tcp = new TcpListener(new IPAddress(
                    new byte[] { 127, 0, 0, 1 }
                ), port);
                tcp.Start();
                
                //Login server
                TcpListener tcpLogin = new TcpListener(new IPAddress(
                    new byte[] { 127, 0, 0, 1 }
                ), loginPort);
                tcpLogin.Start();

                while (true)
                {
                    TcpClient comm = tcp.AcceptTcpClient();
                    TcpClient commLogin = tcpLogin.AcceptTcpClient();

                    lock (_lock) list_clients.Add(count, comm);

                    //new Thread(new Receiver(comm).ReadCallBack).Start(count);
                    new Thread(new Logger(commLogin).ReadLoggerAttempt).Start(count);


                    Thread t = new Thread(handle_clients);
                    t.Start(count);

                    count++;
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    Console.WriteLine("Unexpected disconnection.");
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public static void handle_clients(object o)
        {
            int id = (int) o;
            TcpClient client;

            lock (_lock) client = list_clients[id];
            Logger.ClientToUser.Add(id, 0);

            //Restore current channel msg log
            // ArrayList backlog = main.GetMessageList();
            // foreach(Message message in backlog)
            // {
            //     broadcast(main, message);
            // }

            try{
                while(true)
                {
                    NetworkStream stream = client.GetStream();
                    User_Message message = (User_Message)Net.rcvMsg(stream);
                    Channel channel = null;

                    //Check if message target id = 0
                    //Check if private type id
                    if(message.channel_id == 0 
                        && message.targetID != 0 
                        && message.targetID != Logger.ClientToUser[id])
                    {
                        //redonduncy check
                        foreach(Channel ch in privateChatList)
                        {
                            ArrayList userList = ch.GetUserList();
                            //check if chat already exists
                            if(userList.Contains(message.targetID) 
                                && userList.Contains(Logger.ClientToUser[id]))
                            {
                                channel = ch;
                                Message sysMessage = new User_Message(0, "Chat granted.", channel.id);
                                Net.sendMsg(client.GetStream(), sysMessage);
                                break;
                            }
                        }

                        if(channel == null)
                        {
                            //Check if target user exists
                            if(!Logger.ClientToUser.ContainsValue(message.targetID))
                            {
                                Net.sendMsg(client.GetStream(), new User_Message(0, "Invalid user.", 0));
                            }
                            else{
                                channel = new Channel("private", "private chat");
                                Console.WriteLine("New private channel created: " + channel.id);
                                Net.sendMsg(client.GetStream(), new User_Message(0, "Chat granted.", channel.id));
                                //add to privateChat list so it doesn't get garbage collected with desctructor
                                privateChatList.Add(channel);

                                channel.AddUser(message.targetID);
                                channel.AddUser(Logger.ClientToUser[id]);
                            }
                        }
                    }
                    else if(message.channel_id == 1)
                    {
                        channel = main;
                        if(!channel.GetUserList().Contains(Logger.ClientToUser[id]))
                        {
                            channel.AddUser(Logger.ClientToUser[id]);
                        }
                    }
                    else
                    {
                        foreach(Channel ch in privateChatList)
                        {
                            if(ch.id == message.channel_id)
                            {
                                channel = ch;
                                break;
                            }
                        }
                    }
                    
                    if(channel == null)
                    {
                        continue;
                    }

                    message.channel_id = channel.id;

                    channel.Add(message);
                    //send to channel

                    broadcast(channel, message);
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    Console.WriteLine("Unexpected disconnection.");
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
            
            
            //shutdown the connection, remove record
            lock (_lock) list_clients.Remove(id);
            Logger.ClientToUser.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        public static void broadcast(Channel channel, Message message)
        {
            ArrayList list = channel.GetUserList();

            lock (_lock)
            {
                foreach(int userID in list){
                    int clientKey = -1;
                    //go through dictionnary loop
                    foreach(int key in Logger.ClientToUser.Keys)
                    {
                        if (Logger.ClientToUser[key] == userID)
                        {
                            clientKey = key;
                            break;
                        }
                    }

                    if(clientKey >= 0)
                    {
                        NetworkStream stream = list_clients[clientKey].GetStream();
                        Net.sendMsg(stream, message);
                    }
                }
            }
        }
    }

    class Logger
    {
        private TcpClient comm;
        public Logger(TcpClient comm)
        {
            this.comm = comm;
        }

        public static Dictionary<int, int> ClientToUser = new Dictionary<int, int>();

        public void ReadLoggerAttempt(object o)
        {
            int id = (int) o;
            try
            {
                while (true)
                {
                    if(!comm.Connected)
                    {
                        continue;
                    }
                    User ReceivedUser = (User)Net.rcvUser(comm.GetStream());
                    //Set up new user

                    User user = new User(ReceivedUser.name, ReceivedUser.username, ReceivedUser.email);
                    Console.WriteLine("User Connected: " + "with ID :" + user.id + ", name: "+ user.name);
                    //Attribute to main channel for testing purpose
                    //AsyncSocketServer.main.AddUser(user.id);

                    //Check added user
                    if(AsyncSocketServer.main.ContainsUser(user.id))
                    {
                        Console.WriteLine("User added.");
                    }

                    //Set ClientToUser entry
                    ClientToUser[id] = user.id;


                    //send userBack
                    if(user != null){
                        Net.sendConfirmUserId(comm.GetStream(), user.id);
                    }

                    //Shutdown service
                    if(comm.Connected)
                    {
                        Console.WriteLine("Login process ended, disconnecting from Login service.");
                        comm.Client.Shutdown(SocketShutdown.Both);
                        comm.Close();
                    }
                    
                }
            }
            catch (Exception e)
            {
                if (e is IOException)
                {
                    Console.WriteLine("Unexpected disconnection. ID: " + id);
                }
                else if (e is ObjectDisposedException){
                    Console.WriteLine("Disposed connection. ID: " + id);
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}