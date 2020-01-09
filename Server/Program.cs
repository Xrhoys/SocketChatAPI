using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncSocketServer server = new AsyncSocketServer(8976, 8977);
            server.Start();
        }
    }
}
