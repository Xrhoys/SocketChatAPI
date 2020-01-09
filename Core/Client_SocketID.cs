using System.Collections.Generic;
using System.Collections;
using System.Net.Sockets;

namespace Core
{
    public class Client_SocketID
    {
        public static readonly object _lock = new ArrayList();

        public static readonly Dictionary<int, Socket> list_clients
            = new Dictionary<int, Socket>();

        public static int count = 0;
    }
}