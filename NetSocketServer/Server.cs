using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Communication.Packets;
using Core;
using Communication;


namespace NetSocketServer
{
    // State object for reading client data asynchronously  
    public class StateObject
    {
        // Client  socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 1024;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    public class Server
    {
        private static ManualResetEvent connectDone =
            new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public Server() { }

        public static void StartListening()
        {
            //TODO: SETUP LOGIN SERVER
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // TCP/IP socket

            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            //Test channel main
            Channel main_channel = Channel.CreateNew("main", "test channel for dev");

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                foreach(Channel ch in Channel._channel_list)
                {
                    Console.WriteLine(ch.id);
                }

                while (true)
                {
                    // set semaphore to nonsignaled state
                    connectDone.Reset();

                    //Start async socket to listen for connections

                    Console.WriteLine("Waiting for new connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback), listener
                    );

                    //Wait until a connection is made before continuing
                    connectDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        //Packet receiver
        public static void AcceptCallback(IAsyncResult ar)
        {
            connectDone.Set();

            //Get the socket that handles the client request
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object, replaced with packet object
            StateObject state = new StateObject();
            state.workSocket = handler;
            try
            {
                // Signal the main thread to continue
                
                while (true)
                {
                    receiveDone.Reset();
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    receiveDone.WaitOne();
                }
            }
            catch ( System.Net.Sockets.SocketException e)
            {
                Console.WriteLine("Client disconnected");
                e.ToString();
            }
            catch ( Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        //Dispatcher logic layer
        public static void ReadCallback(IAsyncResult ar)
        {
            try{
            String content = String.Empty;

            //Retrieve the state object and the handler socket
            // from the async state object
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            //Read data from the client socket
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.Length > 0)
                {
                    Packet pck = Packet.Parse(state.buffer);
                    //All the data has been read from the cient. Display it on the console
                    //Console.WriteLine("Read {0} bytes from socket. \n Data : ", content.Length, content);
                    //Business layer:
                    byte[] response = Dispatcher_Server.Switcher(pck);
                    if (response != null && response.Length > 0)
                    {
                        Send(handler, response);
                        sendDone.WaitOne();
                    }
                    if (Dispatcher_Server.SendingQueue.Count > 0)
                    {
                        //TODO: can tweak to accept generalized packet format, instead of specific
                        foreach (Packet sendingPacket in Dispatcher_Server.SendingQueue)
                        {
                            PacketSendMessage messagePacket = (PacketSendMessage)sendingPacket;
                            Send(Client_SocketID.list_clients[User.GetUserByID(messagePacket.userID).socketID],
                                Packet.Encode(messagePacket.PacketType(), (ICanSerialize)messagePacket));
                            sendDone.WaitOne();
                        }
                    }
                    //client/user status as connected (set clientSocketID)
                    else if (Dispatcher_Server.loginPacket?.ID == PacketType.ServerAcknowledgementLogin.ID())
                    {
                        ServerAcknowledgementLogin loginPacket =
                            (ServerAcknowledgementLogin)Dispatcher_Server.loginPacket;
                        Client_SocketID.list_clients.Add(Client_SocketID.count++, handler);
                        User.GetUserByID(loginPacket.id).connectToCurrentClient(Client_SocketID.count);
                    }
                }
                else
                {
                    ///Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            }
            receiveDone.Set();
            }
            catch ( System.Net.Sockets.SocketException e)
            {
                Console.WriteLine("Client disconnected");
                e.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //Packet sender
        public static void Send(Socket handler, byte[] byteData)
        {
            //Convert the string data to byte data using ASCII encoding
            //byte[] byteData = Encoding.ASCII.GetBytes(data);

            //Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallBack), handler);
        }

        // Sending dispatcher
        public static void SendCallBack(IAsyncResult ar)
        {
            try
            {
                //Retrieve the socket from the state object
                Socket handler = (Socket)ar.AsyncState;

                //Complete sending the data to the remote device
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client", bytesSent);

                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}