using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using System.Text;
using Communication.Packets;
using Communication;
using Core;

namespace NetSocketClient
{
    // State object for receiving data from remote device.  
    public class StateObject {  
        // Client socket.  
        public Socket workSocket = null;  
        // Size of receive buffer.  
        public const int BufferSize = 1024;  
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];  
        // Received data string.  
        public StringBuilder sb = new StringBuilder();  
    }  
    
    public class Client {  
        // The port number for the remote device.  
        private const int port = 11000;  
    
        // ManualResetEvent instances signal completion.  
        private static ManualResetEvent connectDone =   
            new ManualResetEvent(false);  
        private static ManualResetEvent sendDone =   
            new ManualResetEvent(false);  
        private static ManualResetEvent receiveDone =   
            new ManualResetEvent(false);  
    
        // The response from the remote device.  
        private static String response = String.Empty;  

        private static Socket client;
    
        public static void StartClient() {  
            // Connect to a remote device.  
            try {  
                // Establish the remote endpoint for the socket.  
                // The name of the   
                // set to localhostd  
                IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");  
                IPAddress ipAddress = ipHostInfo.AddressList[0];  
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);  
    
                // Create a TCP/IP socket.  
                client = new Socket(ipAddress.AddressFamily,  
                    SocketType.Stream, ProtocolType.Tcp);  
    
                // Connect to the remote endpoint.  
                client.BeginConnect( remoteEP,   
                    new AsyncCallback(ConnectCallback), client);  
                connectDone.WaitOne();  
    
                // Send test data to the remote device.
                StateObject state = new StateObject();
                //SET UP THREAD for receive
                new Thread(new ThreadStart(() => {
                    try{
                        while(true)
                        {
                            Receive(client);
                            receiveDone.WaitOne();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                })).Start();



                //TEST SECTION
                ClientAccountRegister register = new ClientAccountRegister();
                register.login = "user";
                register.name = "user";
                register.password = "password";
                Send(client, Packet.Encode(PacketType.ClientAccountRegister, (ICanSerialize)register));
                sendDone.WaitOne();

                // register.login = "user2";
                // register.name = "user2";
                // register.password = "password";
                // Send(client, Packet.Encode(PacketType.ClientAccountRegister, (ICanSerialize)register));
                // sendDone.WaitOne();

                // ClientAccountLogin login = new ClientAccountLogin();
                // login.login = "user";
                // login.password = "password";
                // Send(client, Packet.Encode(PacketType.ClientAccountLogin, (ICanSerialize)login));
                // Console.WriteLine("Sent login.");

                // ClientRequestChannelAccess request = new ClientRequestChannelAccess();
                // request.id = 0;
                // request.SenderID = Session.currentUser.id;
                // Send(client, Packet.Encode(PacketType.ClientRequestChannelAccess, (ICanSerialize)request));
                // sendDone.WaitOne();
                // Console.WriteLine("Sent request.");

                // Receive(client);
                // receiveDone.WaitOne();

                // PacketSendMessage p = new PacketSendMessage();
                // p.ChannelID = 1;
                // p.message = "test";
                // p.userID = 1;
                
                // Send(client, Packet.Encode(PacketType.ClientSendMessage, (ICanSerialize)p));
                // sendDone.WaitOne();




                // Receive(client);
                // receiveDone.WaitOne();

                // Release the socket. 
                Console.WriteLine("ANY KEY TO TERMINATE ...");
                Console.ReadLine(); 
                client.Shutdown(SocketShutdown.Both);  
                client.Close();  
    
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  
    
        private static void ConnectCallback(IAsyncResult ar) {  
            try {  
                // Retrieve the socket from the state object.  
                Socket client = (Socket) ar.AsyncState;  
    
                // Complete the connection.  
                client.EndConnect(ar);  
    
                Console.WriteLine("Socket connected to {0}",  
                    client.RemoteEndPoint.ToString());  
    
                // Signal that the connection has been made.  
                connectDone.Set();  
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  
    
        private static void Receive(Socket client) {  
            try {  
                // Create the state object.  
                StateObject state = new StateObject();  
                state.workSocket = client;  
    
                // Begin receiving the data from the remote device.  
                client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                    new AsyncCallback(ReceiveCallback), state);  
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  
    
        private static void ReceiveCallback( IAsyncResult ar ) {  
            String content = String.Empty;
            try {  
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject) ar.AsyncState;  
                Socket client = state.workSocket;  
    
                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);  
    
                if(bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    content = state.sb.ToString();

                    if(content.Length > 0)
                    {
                        Packet pck = Packet.Parse(state.buffer);

                        byte[] response = Dispatcher_Client.Switcher(pck);

                        if(response != null && response.Length > 0)
                        {
                            Send(client, response);
                        }
                        client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
                            new AsyncCallback(ReceiveCallback), state);  
                    }
                }
                else
                {
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }

                    receiveDone.Set();
                }
                
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }  
    
        private static void Send(Socket client, byte[] data) {  
            // Convert the string data to byte data using ASCII encoding.  
            //byte[] byteData = Encoding.ASCII.GetBytes(data);  
    
            // Begin sending the data to the remote device.  
            client.BeginSend(data, 0, data.Length, 0,  
                new AsyncCallback(SendCallback), client);  
        }  
    
        private static void SendCallback(IAsyncResult ar) {  
            try {  
                // Retrieve the socket from the state object.  
                Socket client = (Socket) ar.AsyncState;  
    
                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);  
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);
    
                // Signal that all bytes have been sent.  
                sendDone.Set();  
            } catch (Exception e) {  
                Console.WriteLine(e.ToString());  
            }  
        }
    }
}