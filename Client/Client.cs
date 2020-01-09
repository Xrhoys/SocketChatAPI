using System;
using System.Net.Sockets;
using Communication;
using System.IO;
using System.Threading;

namespace Client
{
    public class AsyncSocketClient
    {
        public static ManualResetEvent sendDone = new ManualResetEvent(false);

        public static ManualResetEvent receiveDone = new ManualResetEvent(false);

        public static ManualResetEvent MenuDone = new ManualResetEvent(false);

        private string hostname;
        private int port;
        private int loginPort;

        private static TcpClient comm;

        public static Channel main = new Channel("main", "dev test channel");

        public static int current_channel = 0;

        public AsyncSocketClient(string hostname, int port, int loginPort)
        {
            this.hostname = hostname;
            this.port = port;
            this.loginPort = loginPort;
        }

        public void Start()
        {
            try
            {
                //Authentification
                TcpClient commLogin = new TcpClient(hostname, loginPort);
                Console.WriteLine("Connected to Login Server.");
                Console.WriteLine("Enter new username:");

                //Start identification server
                new Thread(new Logger(commLogin).SetUserID).Start();

                //Attempt to log in
                while (!new Logger(commLogin).LoggerCallBack())
                {
                    //Connect to server as Username
                }

                

                comm = new TcpClient(hostname, port);
                Console.WriteLine("Connnected to chat service.");

                Menu();
                MenuDone.WaitOne();

                new Thread(new Receiver(comm).SendCallBack).Start();
                new Thread(new Receiver(comm).ReadCallBack).Start();
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

        public static void RequestPrivate()
        {
            //Request user input for UserID
            Console.WriteLine("Request user ID for chat");
            int request_ID = Int32.Parse(Console.ReadLine());

            //Initiate conversation
            Message message = new User_Message(Logger.GetCurrentUser(), "Request Private Chat", 0);
            message.SetTargetID(request_ID);

            Net.sendMsg(comm.GetStream(), message);
            comm.GetStream().Flush();
        }

        public void Close()
        {
            comm.GetStream().Close();
            comm.Close();
            comm.Dispose();
        }

        public static void Menu()
        {
            Console.WriteLine("1. Main text channel.");
            Console.WriteLine("2. Private message.");
            string str = Console.ReadLine();
            switch(str)
            {
                case "1": current_channel = 1;
                    break;
                case "2": RequestPrivate();
                    break;
                default:
                    Console.WriteLine("Wrong choice, please restart application.");
                    break;
            }
            MenuDone.Set();
        }
    }

    class Receiver
    {
        private TcpClient comm;

        public Receiver(TcpClient s)
        {
            comm = s;
        }

        public void SendCallBack()
        {
            try
            {
                while (true)
                {
                    string str = Console.ReadLine();

                    if(str == String.Empty)
                    {
                        Console.WriteLine("Message cannot be empty");
                        continue;
                    }
                    if(AsyncSocketClient.current_channel == 0)
                    {
                        Console.WriteLine("No channel selected, abort.");
                        continue;
                    }

                    //Set message
                    Message message = new User_Message(Logger.GetCurrentUser(), str, AsyncSocketClient.current_channel);
                    Net.sendMsg(comm.GetStream(), message);

                    comm.GetStream().FlushAsync();
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

        public void ReadCallBack()
        {
            try{
                while (true)
                {
                    User_Message message = (User_Message)Net.rcvMsg(comm.GetStream());
                    if(message.channel_id != 0)
                    {
                        AsyncSocketClient.current_channel = message.channel_id;
                    }

                    //check if the message is 
                    Console.WriteLine(message);

                    comm.GetStream().FlushAsync();
                }
            } catch (Exception e)
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
    }

    class Logger
    {
        private static TcpClient comm;

        private static int userID = 0;

        public Logger(TcpClient tcp)
        {
            comm = tcp;
        }

        public bool LoggerCallBack()
        {
            try
            {
                while(true){
                    string str = Console.ReadLine();

                    User user = new User(str, "default", "default@email.net");

                    Net.sendUser(comm.GetStream(), user);

                    comm.GetStream().Flush();

                    return true;
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
            return false;
        }

        public static int GetCurrentUser()
        {
            return userID;
        }

        public void SetUserID()
        {
            try
            {
                while(userID == 0)
                {
                    userID = Net.rcvConfirmUserId(comm.GetStream());
                    Console.WriteLine("Connected as: " + userID + "\nEnding login phase.");
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
    }
}